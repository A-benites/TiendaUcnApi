using Microsoft.EntityFrameworkCore;
using TiendaUcnApi.src.Domain.Models;
using TiendaUcnApi.src.Infrastructure.Data;
using TiendaUcnApi.src.Infrastructure.Repositories.Interfaces;

namespace TiendaUcnApi.src.Infrastructure.Repositories.Implements;

/// <summary>
/// Implementación del repositorio de archivos de imagen.
/// </summary>
public class FileRepository : IFileRepository
{
    /// <summary>
    /// Contexto de base de datos de la aplicación.
    /// </summary>
    private readonly AppDbContext _context;

    /// <summary>
    /// Inicializa una nueva instancia del repositorio de archivos.
    /// </summary>
    /// <param name="context">Contexto de base de datos.</param>
    public FileRepository(AppDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Crea un archivo de imagen en la base de datos.
    /// </summary>
    /// <param name="file">El archivo de imagen a crear.</param>
    /// <returns>True si el archivo se creó correctamente, de lo contrario false y null en caso de que la imagen ya existe.</returns>
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
    /// Elimina un archivo de imagen de la base de datos.
    /// </summary>
    /// <param name="publicId">El identificador público del archivo a eliminar.</param>
    /// <returns>True si el archivo se eliminó correctamente, de lo contrario false y null si la imagen no existe.</returns>
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
    /// Obtiene una imagen por su ID.
    /// </summary>
    /// <param name="imageId">ID de la imagen.</param>
    /// <returns>La imagen encontrada o null si no existe.</returns>
    public async Task<Image?> GetImageByIdAsync(int imageId)
    {
        return await _context.Images.AsNoTracking().FirstOrDefaultAsync(i => i.Id == imageId);
    }
}