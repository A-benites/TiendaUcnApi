using System.ComponentModel.DataAnnotations;
using TiendaUcnApi.src.Domain.Models;

namespace TiendaUcnApi.src.Application.DTO.ProductDTO;

/// <summary>
/// DTO para la creación de un nuevo producto.
/// </summary>
public class ProductCreateDTO
{
    /// <summary>
    /// Título del producto.
    /// </summary>
    [Required(ErrorMessage = "El título del producto es obligatorio.")]
    [StringLength(50, ErrorMessage = "El título no puede exceder los 50 caracteres.")]
    [MinLength(3, ErrorMessage = "El título debe tener al menos 3 caracteres.")]
    public required string Title { get; set; }

    /// <summary>
    /// Descripción del producto.
    /// </summary>
    [Required(ErrorMessage = "La descripción del producto es obligatoria.")]
    [StringLength(100, ErrorMessage = "La descripción no puede exceder los 100 caracteres.")]
    [MinLength(10, ErrorMessage = "La descripción debe tener al menos 10 caracteres.")]
    public required string Description { get; set; }

    /// <summary>
    /// Precio del producto.
    /// </summary>
    [Required(ErrorMessage = "El precio del producto es obligatorio.")]
    [Range(0, int.MaxValue, ErrorMessage = "El precio debe ser un valor entero positivo.")]
    public required int Price { get; set; }

    /// <summary>
    /// Porcentaje de descuento del producto.
    /// </summary>
    [Required(ErrorMessage = "El descuento del producto es obligatorio.")]
    [Range(0, 100, ErrorMessage = "El descuento debe estar entre 0 y 100.")]
    public required int Discount { get; set; }

    /// <summary>
    /// Stock disponible del producto.
    /// </summary>
    [Required(ErrorMessage = "El stock del producto es obligatorio.")]
    [Range(1, int.MaxValue, ErrorMessage = "El stock debe ser un valor positivo.")]
    public required int Stock { get; set; }

    /// <summary>
    /// Estado del producto (Nuevo, Usado).
    /// </summary>
    [Required(ErrorMessage = "El estado del producto es obligatorio.")]
    [RegularExpression("^(New|Used)$", ErrorMessage = "El estado debe ser 'Nuevo' o 'Usado'.")]
    public required Status Status { get; set; }

    /// <summary>
    /// Nombre de la categoría del producto.
    /// </summary>
    [Required(ErrorMessage = "El nombre de la categoría es obligatorio.")]
    [StringLength(50, ErrorMessage = "El nombre de la categoría no puede exceder los 50 caracteres.")]
    [MinLength(3, ErrorMessage = "El nombre de la categoría debe tener al menos 3 caracteres.")]
    public required string CategoryName { get; set; }

    /// <summary>
    /// Nombre de la marca del producto.
    /// </summary>
    [Required(ErrorMessage = "El nombre de la marca es obligatorio.")]
    [StringLength(50, ErrorMessage = "El nombre de la marca no puede exceder los 50 caracteres.")]
    [MinLength(3, ErrorMessage = "El nombre de la marca debe tener al menos 3 caracteres.")]
    public required string BrandName { get; set; }

    /// <summary>
    /// Imágenes del producto.
    /// </summary>
    [Required(ErrorMessage = "Las imágenes del producto son obligatorias.")]
    public required List<IFormFile> Images { get; set; } = new List<IFormFile>();
}