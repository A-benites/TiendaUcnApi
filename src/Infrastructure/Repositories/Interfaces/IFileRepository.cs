using TiendaUcnApi.src.Domain.Models;

namespace TiendaUcnApi.src.Infrastructure.Repositories.Interfaces;

/// <summary>
/// Repository interface for image file data access operations.
/// Manages database records of images stored in Cloudinary CDN.
/// </summary>
public interface IFileRepository
{
    /// <summary>
    /// Creates an image record in the database.
    /// </summary>
    /// <param name="file">The image entity to create.</param>
    /// <returns>True if created successfully, false if creation failed, null if the image already exists.</returns>
    Task<bool?> CreateAsync(Image file);

    /// <summary>
    /// Deletes an image record from the database by its public ID.
    /// </summary>
    /// <param name="publicId">The Cloudinary public identifier of the image.</param>
    /// <returns>True if deleted successfully, false if deletion failed, null if the image doesn't exist.</returns>
    Task<bool?> DeleteAsync(string publicId);

    /// <summary>
    /// Retrieves an image by its database identifier.
    /// </summary>
    /// <param name="imageId">The image identifier.</param>
    /// <returns>Image entity or null if not found.</returns>
    Task<Image?> GetImageByIdAsync(int imageId);
}
