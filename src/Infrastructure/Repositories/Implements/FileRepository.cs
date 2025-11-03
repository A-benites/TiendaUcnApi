using Microsoft.EntityFrameworkCore;
using TiendaUcnApi.src.Domain.Models;
using TiendaUcnApi.src.Infrastructure.Data;
using TiendaUcnApi.src.Infrastructure.Repositories.Interfaces;

namespace TiendaUcnApi.src.Infrastructure.Repositories.Implements;

/// <summary>
/// Implementation of the image file repository.
/// Manages product image records in the database.
/// </summary>
public class FileRepository : IFileRepository
{
    /// <summary>
    /// Application database context.
    /// </summary>
    private readonly AppDbContext _context;

    /// <summary>
    /// Initializes a new instance of the <see cref="FileRepository"/> class.
    /// </summary>
    /// <param name="context">Database context.</param>
    public FileRepository(AppDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Creates an image file record in the database.
    /// </summary>
    /// <param name="file">The image file to create.</param>
    /// <returns>True if the file was created successfully, null if the image already exists, false otherwise.</returns>
    public async Task<bool?> CreateAsync(Image file)
    {
        var existsImage = await _context.Images.AnyAsync(i => i.PublicId == file.PublicId);
        if (!existsImage)
        {
            _context.Images.Add(file);
            return await _context.SaveChangesAsync() > 0;
        }
        return null;
    }

    /// <summary>
    /// Deletes an image file from the database.
    /// </summary>
    /// <param name="publicId">The public identifier of the file to delete.</param>
    /// <returns>True if the file was deleted successfully, null if the image doesn't exist, false otherwise.</returns>
    public async Task<bool?> DeleteAsync(string publicId)
    {
        var image = await _context.Images.FirstOrDefaultAsync(i => i.PublicId == publicId);
        if (image != null)
        {
            _context.Images.Remove(image);
            return await _context.SaveChangesAsync() > 0;
        }
        return null;
    }

    /// <summary>
    /// Retrieves an image by its ID.
    /// </summary>
    /// <param name="imageId">Image ID.</param>
    /// <returns>The found image or null if it doesn't exist.</returns>
    public async Task<Image?> GetImageByIdAsync(int imageId)
    {
        return await _context.Images.AsNoTracking().FirstOrDefaultAsync(i => i.Id == imageId);
    }
}
