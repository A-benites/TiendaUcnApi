using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Serilog;
using SkiaSharp;
using TiendaUcnApi.src.Application.Services.Interfaces;
using TiendaUcnApi.src.Domain.Models;
using TiendaUcnApi.src.Infrastructure.Repositories.Interfaces;

namespace TiendaUcnApi.src.Application.Services.Implements;

/// <summary>
/// Service for handling file operations with Cloudinary.
/// Manages image uploads, deletions, and transformations for product images.
/// </summary>
public class FileService : IFileService
{
    /// <summary>
    /// Application configuration.
    /// </summary>
    private readonly IConfiguration _configuration;

    /// <summary>
    /// Cloudinary instance for cloud operations.
    /// </summary>
    private readonly Cloudinary _cloudinary;

    /// <summary>
    /// Allowed file extensions for images.
    /// </summary>
    private readonly string[] _allowedExtensions;

    /// <summary>
    /// Maximum allowed file size for images (in bytes).
    /// </summary>
    private readonly int _maxFileSizeInBytes;

    /// <summary>
    /// File repository.
    /// </summary>
    private readonly IFileRepository _fileRepository;

    /// <summary>
    /// Cloudinary cloud name.
    /// </summary>
    private readonly string _cloudName;

    /// <summary>
    /// Cloudinary API Key.
    /// </summary>
    private readonly string _cloudApiKey;

    /// <summary>
    /// Cloudinary API Secret.
    /// </summary>
    private readonly string _cloudApiSecret;

    /// <summary>
    /// Image transformation width.
    /// </summary>
    private readonly int _transformationWidth;

    /// <summary>
    /// Crop type for image transformation.
    /// </summary>
    private readonly string _transformationCrop;

    /// <summary>
    /// Quality for image transformation.
    /// </summary>
    private readonly string _transformationQuality;

    /// <summary>
    /// Image format for transformation.
    /// </summary>
    private readonly string _transformationFetchFormat;

    /// <summary>
    /// Initializes a new instance of the FileService class with all necessary dependencies.
    /// Configures Cloudinary client and image transformation settings from configuration.
    /// </summary>
    /// <param name="configuration">Application configuration.</param>
    /// <param name="fileRepository">File repository.</param>
    /// <exception cref="InvalidOperationException">Thrown when required configuration values are missing.</exception>
    public FileService(IConfiguration configuration, IFileRepository fileRepository)
    {
        _configuration = configuration;
        _fileRepository = fileRepository;
        _cloudName =
            _configuration["Cloudinary:CloudName"]
            ?? throw new InvalidOperationException("CloudName configuration is required");
        _cloudApiKey =
            _configuration["Cloudinary:ApiKey"]
            ?? throw new InvalidOperationException("ApiKey configuration is required");
        _cloudApiSecret =
            _configuration["Cloudinary:ApiSecret"]
            ?? throw new InvalidOperationException("ApiSecret configuration is required");
        Account account = new Account(_cloudName, _cloudApiKey, _cloudApiSecret);
        _cloudinary = new Cloudinary(account);
        _cloudinary.Api.Secure = true;
        _allowedExtensions =
            _configuration.GetSection("Products:ImageAllowedExtensions").Get<string[]>()
            ?? throw new InvalidOperationException("Image extensions configuration is required");
        _transformationQuality =
            _configuration["Products:TransformationQuality"]
            ?? throw new InvalidOperationException(
                "Transformation quality configuration is required"
            );
        _transformationCrop =
            _configuration["Products:TransformationCrop"]
            ?? throw new InvalidOperationException("Transformation crop configuration is required");
        _transformationFetchFormat =
            _configuration["Products:TransformationFetchFormat"]
            ?? throw new InvalidOperationException(
                "Transformation format configuration is required"
            );
        if (!int.TryParse(_configuration["Products:ImageMaxSizeInBytes"], out _maxFileSizeInBytes))
        {
            throw new InvalidOperationException("Image size configuration is required");
        }
        if (!int.TryParse(_configuration["Products:TransformationWidth"], out _transformationWidth))
        {
            throw new InvalidOperationException("Transformation width configuration is required");
        }
    }

    /// <summary>
    /// Uploads a file to Cloudinary.
    /// Validates file size, extension, and image format before uploading.
    /// Applies transformation (resize, crop, quality, format) during upload for optimization.
    /// </summary>
    /// <param name="file">The file to upload.</param>
    /// <param name="productId">The ID of the product to which the image belongs.</param>
    /// <returns>True if upload was successful, false if image already exists.</returns>
    /// <exception cref="ArgumentException">Thrown when productId is invalid, file is invalid, exceeds size limit, has invalid extension, or is not a valid image.</exception>
    /// <exception cref="Exception">Thrown when upload fails or database operation fails.</exception>
    public async Task<bool> UploadAsync(IFormFile file, int productId)
    {
        if (productId <= 0)
        {
            Log.Error($"Invalid ProductId: {productId}");
            throw new ArgumentException("ProductId must be greater than 0");
        }

        if (file == null || file.Length == 0)
        {
            Log.Error("Attempt to upload a null or empty file");
            throw new ArgumentException("Invalid file");
        }

        if (file.Length > _maxFileSizeInBytes)
        {
            Log.Error(
                $"File {file.FileName} exceeds the maximum allowed size of {_maxFileSizeInBytes / 1024 / 1024} MB"
            );
            throw new ArgumentException(
                $"File exceeds the maximum allowed size of {_maxFileSizeInBytes / 1024 / 1024} MB"
            );
        }

        var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();

        if (!_allowedExtensions.Contains(fileExtension))
        {
            Log.Error($"File extension not allowed: {fileExtension}");
            throw new ArgumentException(
                $"File extension not allowed. Allowed: {string.Join(", ", _allowedExtensions)}"
            );
        }

        if (!IsValidImageFile(file))
        {
            Log.Error($"File {file.FileName} is not a valid image");
            throw new ArgumentException("File is not a valid image");
        }
        var folder = $"product/{productId}/images";
        using var stream = file.OpenReadStream();

        var uploadParams = new ImageUploadParams()
        {
            Folder = folder,
            File = new FileDescription(file.FileName, stream),
            UseFilename = true,
            UniqueFilename = true,
        };

        Log.Information($"Optimizing image: {file.FileName} before uploading to cloud");
        uploadParams.Transformation = new Transformation()
            .Width(_transformationWidth)
            .Crop(_transformationCrop)
            .Chain()
            .Quality(_transformationQuality)
            .Chain()
            .FetchFormat(_transformationFetchFormat);

        Log.Information($"Uploading image: {file.FileName} to Cloudinary");
        var uploadResult = await _cloudinary.UploadAsync(uploadParams);

        if (uploadResult.Error != null)
        {
            Log.Error($"Error uploading image: {uploadResult.Error.Message}");
            throw new Exception($"Error uploading image: {uploadResult.Error.Message}");
        }

        var image = new Image()
        {
            PublicId = uploadResult.PublicId,
            ImageUrl = uploadResult.SecureUrl.ToString(),
            ProductId = productId,
        };

        var result = await _fileRepository.CreateAsync(image);
        if (result is bool && !result.Value!)
        {
            Log.Error($"Error saving image to database: {file.FileName}");
            var deleteResult = await DeleteInCloudinaryAsync(uploadResult.PublicId);
            if (!deleteResult)
            {
                Log.Error(
                    $"Error deleting image from Cloudinary after database creation failed: {uploadResult.PublicId}"
                );
                throw new Exception(
                    "Error deleting image from Cloudinary after database creation failed"
                );
            }
            throw new Exception("Error saving image to database");
        }
        else if (result is null)
        {
            Log.Warning($"Image already exists in database: {file.FileName}");
            return false;
        }

        Log.Information($"Image uploaded successfully: {uploadResult.SecureUrl}");
        return true;
    }

    /// <summary>
    /// Deletes a file from Cloudinary and the database.
    /// </summary>
    /// <param name="publicId">The public ID of the file to delete.</param>
    /// <returns>True if deletion was successful, false if image doesn't exist in database.</returns>
    /// <exception cref="Exception">Thrown when deletion from Cloudinary or database fails.</exception>
    public async Task<bool> DeleteAsync(string publicId)
    {
        var deletionParams = new DeletionParams(publicId);
        Log.Information($"Deleting image with PublicId: {publicId} from Cloudinary");
        var deleteResult = await _cloudinary.DestroyAsync(deletionParams);
        if (deleteResult.Error != null)
        {
            Log.Error(
                $"Error deleting image with PublicId: {publicId} from Cloudinary: {deleteResult.Error.Message}"
            );
            throw new Exception($"Error deleting image: {deleteResult.Error.Message}");
        }
        Log.Information($"Image with PublicId: {publicId} successfully deleted from Cloudinary");
        var result = await _fileRepository.DeleteAsync(publicId);
        if (result is bool && !result.Value!)
        {
            Log.Error($"Error deleting image from database with PublicId: {publicId}");
            throw new Exception("Error deleting image from database");
        }
        else if (result is null)
        {
            Log.Warning($"Image does not exist in database with PublicId: {publicId}");
            return false;
        }
        Log.Information($"Image with PublicId: {publicId} successfully deleted from database");
        return true;
    }

    /// <summary>
    /// Deletes an image from Cloudinary asynchronously.
    /// Internal helper method used for rollback operations.
    /// </summary>
    /// <param name="publicId">The public ID of the image to delete.</param>
    /// <returns>True if deletion was successful, false otherwise.</returns>
    private async Task<bool> DeleteInCloudinaryAsync(string publicId)
    {
        var deletionParams = new DeletionParams(publicId);
        Log.Information($"Deleting image with PublicId: {publicId} from Cloudinary");
        var deleteResult = await _cloudinary.DestroyAsync(deletionParams);
        if (deleteResult.Error != null)
        {
            Log.Error(
                $"Error deleting image with PublicId: {publicId} from Cloudinary: {deleteResult.Error.Message}"
            );
            return false;
        }
        Log.Information($"Image with PublicId: {publicId} successfully deleted from Cloudinary");
        return true;
    }

    /// <summary>
    /// Validates if the file is a valid image.
    /// Uses SkiaSharp to decode and verify image format.
    /// </summary>
    /// <param name="file">The file to validate.</param>
    /// <returns>True if the file is a valid image, false otherwise.</returns>
    private bool IsValidImageFile(IFormFile file)
    {
        try
        {
            using var stream = file.OpenReadStream();
            using var skiaStream = new SKManagedStream(stream);
            using var codec = SKCodec.Create(skiaStream);

            return codec != null && codec.Info.Width > 0 && codec.Info.Height > 0;
        }
        catch (Exception ex)
        {
            Log.Warning($"Error validating image {file.FileName}: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// Deletes an image by its ID.
    /// </summary>
    /// <param name="imageId">Image ID.</param>
    /// <returns>True if deletion was successful, false if image doesn't exist.</returns>
    /// <exception cref="KeyNotFoundException">Thrown when image doesn't exist.</exception>
    public async Task<bool> DeleteAsync(int imageId)
    {
        var image =
            await _fileRepository.GetImageByIdAsync(imageId)
            ?? throw new KeyNotFoundException("The image does not exist.");

        return await DeleteAsync(image.PublicId);
    }
}
