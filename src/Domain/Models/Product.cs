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
    // Añade esta data annotation a Description
    [MinLength(20, ErrorMessage = "La descripción debe tener al menos 20 caracteres.")]
    public string Description { get; set; }

    /// <summary>
    /// Precio del producto.
    /// </summary>
    // La columna no puede ser nula.
    [Required]
    // Define el tipo de dato en la BD para mayor precisión con valores monetarios.
    [Range(0.01, (double)decimal.MaxValue, ErrorMessage = "El precio debe ser un valor positivo.")]
    public decimal Price { get; set; }

    /// <summary>
    /// Descuento del producto.
    /// </summary>
    // Añade esta data annotation a Discount para evitar valores negativos
    [Range(0, (double)decimal.MaxValue, ErrorMessage = "El descuento no puede ser negativo.")]
    public decimal Discount { get; set; }

    /// <summary>
    /// Stock del producto.
    /// </summary>
    // La columna no puede ser nula.
    [Required]
    [Range(0, int.MaxValue)]
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