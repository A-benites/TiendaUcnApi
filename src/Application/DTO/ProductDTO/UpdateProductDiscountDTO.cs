using System.ComponentModel.DataAnnotations;

namespace TiendaUcnApi.src.Application.DTO.ProductDTO;

/// <summary>
/// DTO para actualizar el descuento de un producto.
/// </summary>
public class UpdateProductDiscountDTO
{
    /// <summary>
    /// Porcentaje de descuento a aplicar (0-100).
    /// </summary>
    [Required(ErrorMessage = "El descuento es obligatorio.")]
    [Range(0, 100, ErrorMessage = "El descuento debe estar entre 0 y 100.")]
    public int Discount { get; set; }
}