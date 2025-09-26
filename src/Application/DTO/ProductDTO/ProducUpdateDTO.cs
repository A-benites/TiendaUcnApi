using System.ComponentModel.DataAnnotations;
using TiendaUcnApi.src.Domain.Models;

namespace TiendaUcnApi.src.Application.DTO.ProductDTO;

/// <summary>
/// DTO para actualizar los datos de un producto.
/// </summary>
public class ProducUpdateDTO
{
    /// <summary>
    /// Título del producto.
    /// </summary>
    [StringLength(50, MinimumLength = 3)]
    public string? Title { get; set; }

    /// <summary>
    /// Descripción del producto.
    /// </summary>
    [MinLength(10)]
    public string? Description { get; set; }

    /// <summary>
    /// Precio del producto.
    /// </summary>
    [Range(1, int.MaxValue)]
    public int? Price { get; set; }

    /// <summary>
    /// Porcentaje de descuento del producto.
    /// </summary>
    [Range(0, 100)]
    public int? Discount { get; set; }

    /// <summary>
    /// Stock disponible del producto.
    /// </summary>
    [Range(0, int.MaxValue)]
    public int? Stock { get; set; }

    /// <summary>
    /// Estado del producto (Nuevo, Usado, etc.).
    /// </summary>
    public Status? Status { get; set; }
}