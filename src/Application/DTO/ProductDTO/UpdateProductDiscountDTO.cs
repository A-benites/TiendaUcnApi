using System.ComponentModel.DataAnnotations;

namespace TiendaUcnApi.src.Application.DTO.ProductDTO;

public class UpdateProductDiscountDTO
{
    [Required(ErrorMessage = "El descuento es obligatorio.")]
    [Range(0, 100, ErrorMessage = "El descuento debe estar entre 0 y 100.")]
    public int Discount { get; set; }
}