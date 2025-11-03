using System.ComponentModel.DataAnnotations;
using TiendaUcnApi.src.Domain.Models;

namespace TiendaUcnApi.src.Application.DTO.ProductDTO;

/// <summary>
/// Data Transfer Object for creating a new product.
/// </summary>
public class ProductCreateDTO
{
    /// <summary>
    /// Product title. Must be between 3 and 50 characters.
    /// </summary>
    [Required(ErrorMessage = "El título del producto es obligatorio.")]
    [StringLength(50, ErrorMessage = "El título no puede exceder los 50 caracteres.")]
    [MinLength(3, ErrorMessage = "El título debe tener al menos 3 caracteres.")]
    public required string Title { get; set; }

    /// <summary>
    /// Product description. Must be between 10 and 100 characters.
    /// </summary>
    [Required(ErrorMessage = "La descripción del producto es obligatoria.")]
    [StringLength(100, ErrorMessage = "La descripción no puede exceder los 100 caracteres.")]
    [MinLength(10, ErrorMessage = "La descripción debe tener al menos 10 caracteres.")]
    public required string Description { get; set; }

    /// <summary>
    /// Product price. Must be a positive integer.
    /// </summary>
    [Required(ErrorMessage = "El precio del producto es obligatorio.")]
    [Range(0, int.MaxValue, ErrorMessage = "El precio debe ser un valor entero positivo.")]
    public required int Price { get; set; }

    /// <summary>
    /// Product discount percentage. Must be between 0 and 100.
    /// </summary>
    [Required(ErrorMessage = "El descuento del producto es obligatorio.")]
    [Range(0, 100, ErrorMessage = "El descuento debe estar entre 0 y 100.")]
    public required int Discount { get; set; }

    /// <summary>
    /// Available stock quantity. Must be a positive value.
    /// </summary>
    [Required(ErrorMessage = "El stock del producto es obligatorio.")]
    [Range(1, int.MaxValue, ErrorMessage = "El stock debe ser un valor positivo.")]
    public required int Stock { get; set; }

    /// <summary>
    /// Product status (New or Used).
    /// </summary>
    [Required(ErrorMessage = "El estado del producto es obligatorio.")]
    [RegularExpression("^(New|Used)$", ErrorMessage = "El estado debe ser 'Nuevo' o 'Usado'.")]
    public required Status Status { get; set; }

    /// <summary>
    /// Category ID for the product.
    /// </summary>
    [Required(ErrorMessage = "El ID de la categoría es obligatorio.")]
    public required int CategoryId { get; set; }

    /// <summary>
    /// Brand ID for the product.
    /// </summary>
    [Required(ErrorMessage = "El ID de la marca es obligatorio.")]
    public required int BrandId { get; set; }

    /// <summary>
    /// Product images to upload.
    /// </summary>
    [Required(ErrorMessage = "Las imágenes del producto son obligatorias.")]
    public required List<IFormFile> Images { get; set; } = new List<IFormFile>();
}
