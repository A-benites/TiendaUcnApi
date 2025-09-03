using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TiendaUcnApi.src.Domain.Models;

public class Image
{
    /// <summary>
    /// Identificador único de la imagen.
    /// </summary>
    [Key]
    public int Id { get; set; }

    /// <summary>
    /// URL pública de la imagen alojada en el servicio externo (ej. Cloudinary).
    /// </summary>
    // La columna en la base de datos no puede ser nula.
    [Required]
    public string ImageUrl { get; set; }

    /// <summary>
    /// Identificador público usado por el servicio externo para gestionar la imagen.
    /// </summary>
    // La columna en la base de datos no puede ser nula.
    [Required]
    public string PublicId { get; set; }

    /// <summary>
    /// Fecha de creación del registro de la imagen.
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // --- Relación con Product ---

    /// <summary>
    /// Identificador del producto al que pertenece esta imagen.
    /// </summary>
    public int ProductId { get; set; }

    /// <summary>
    /// Producto asociado a la imagen (propiedad de navegación).
    /// </summary>
    // Define la clave foránea para la relación con la tabla Product.
    [ForeignKey("ProductId")]
    public Product Product { get; set; } = null!;
}