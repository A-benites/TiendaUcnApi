namespace TiendaUcnApi.src.Application.Services.Interfaces;

/// <summary>
/// Service interface for file and image management using Cloudinary CDN.
/// </summary>
public interface IFileService
{
    /// <summary>
    /// Uploads an image file to Cloudinary and associates it with a product.
    /// </summary>
    /// <param name="file">The image file to upload.</param>
    /// <param name="productId">The product identifier to associate with the image.</param>
    /// <returns>True if upload was successful, false otherwise.</returns>
    Task<bool> UploadAsync(IFormFile file, int productId);

    /// <summary>
    /// Deletes an image from Cloudinary using its public ID.
    /// </summary>
    /// <param name="publicId">The Cloudinary public ID of the image to delete.</param>
    /// <returns>True if deletion was successful, false otherwise.</returns>
    Task<bool> DeleteAsync(string publicId);

    /// <summary>
    /// Deletes an image from Cloudinary and database using the database image ID.
    /// </summary>
    /// <param name="imageId">The database identifier of the image to delete.</param>
    /// <returns>True if deletion was successful, false otherwise.</returns>
    Task<bool> DeleteAsync(int imageId);
}
