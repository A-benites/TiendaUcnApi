using System.ComponentModel.DataAnnotations;

namespace TiendaUcnApi.src.Application.DTO.ProductDTO;

/// <summary>
/// Data Transfer Object for updating a product's discount.
/// </summary>
public class UpdateProductDiscountDTO
{
    /// <summary>
    /// Discount percentage to apply (0-100).
    /// </summary>
    [Required(ErrorMessage = "El descuento es obligatorio.")]
    [Range(0, 100, ErrorMessage = "El descuento debe estar entre 0 y 100.")]
    public int Discount { get; set; }
}
