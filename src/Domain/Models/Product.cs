using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TiendaUcnApi.src.Domain.Models;

public enum Status
{
    New,
    Used,
}

public class Product
{
    /// <summary>
    /// Identificador único del producto.
    /// </summary>
    [Key]
    public int Id { get; set; }

    /// <summary>
    /// Título del producto.
    /// </summary>
    // La columna no puede ser nula.
    [Required]
    // Define el largo máximo de 50 y mínimo de 3 caracteres.
    [StringLength(50, MinimumLength = 3)]
    public string Title { get; set; }

    /// <summary>
    /// Descripción del producto.
    /// </summary>
    // La columna no puede ser nula.
    [Required]
    public string Description { get; set; }

    /// <summary>
    /// Precio del producto.
    /// </summary>
    // La columna no puede ser nula.
    [Required]
    // Define el tipo de dato en la BD para mayor precisión con valores monetarios.
    [Column(TypeName = "decimal(18,2)")]
    public decimal Price { get; set; }

    /// <summary>
    /// Descuento del producto.
    /// </summary>
    // Define el tipo de dato en la BD para mayor precisión.
    [Column(TypeName = "decimal(18,2)")]
    public decimal Discount { get; set; }

    /// <summary>
    /// Stock del producto.
    /// </summary>
    // La columna no puede ser nula.
    [Required]
    public int Stock { get; set; }

    /// <summary>
    /// Estado del producto (Nuevo o usado).
    /// </summary>
    // La columna no puede ser nula.
    [Required]
    public Status Status { get; set; }

    /// <summary>
    /// Indica si el producto está lógicamente eliminado (no se muestra al cliente).
    /// </summary>
    public bool IsAvailable { get; set; } = true;

    /// <summary>
    /// Fecha de creación del producto.
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Fecha de actualización del producto.
    /// </summary>
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // --- Relaciones con otras tablas ---

    /// <summary>
    /// Identificador de la categoría del producto.
    /// </summary>
    public int CategoryId { get; set; }

    /// <summary>
    /// Categoría del producto (propiedad de navegación).
    /// </summary>
    [ForeignKey("CategoryId")]
    public Category Category { get; set; } = null!;

    /// <summary>
    /// Identificador de la marca del producto.
    /// </summary>
    public int BrandId { get; set; }

    /// <summary>
    /// Marca del producto (propiedad de navegación).
    /// </summary>
    [ForeignKey("BrandId")]
    public Brand Brand { get; set; } = null!;

    /// <summary>
    /// Lista de imágenes asociadas al producto.
    /// </summary>
    public ICollection<Image> Images { get; set; } = new List<Image>();
}