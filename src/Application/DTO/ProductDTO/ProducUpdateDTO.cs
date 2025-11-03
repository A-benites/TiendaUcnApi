using System.ComponentModel.DataAnnotations;
using TiendaUcnApi.src.Domain.Models;

namespace TiendaUcnApi.src.Application.DTO.ProductDTO;

/// <summary>
/// Data Transfer Object for updating product data.
/// </summary>
public class ProducUpdateDTO
{
    /// <summary>
    /// Product title (optional). Must be between 3 and 50 characters if provided.
    /// </summary>
    [StringLength(50, MinimumLength = 3)]
    public string? Title { get; set; }

    /// <summary>
    /// Product description (optional). Must be at least 10 characters if provided.
    /// </summary>
    [MinLength(10)]
    public string? Description { get; set; }

    /// <summary>
    /// Product price (optional). Must be a positive value if provided.
    /// </summary>
    [Range(1, int.MaxValue)]
    public int? Price { get; set; }

    /// <summary>
    /// Product discount percentage (optional). Must be between 0 and 100 if provided.
    /// </summary>
    [Range(0, 100)]
    public int? Discount { get; set; }

    /// <summary>
    /// Available stock quantity (optional). Must be a non-negative value if provided.
    /// </summary>
    [Range(0, int.MaxValue)]
    public int? Stock { get; set; }

    /// <summary>
    /// Product status (optional). Can be New or Used.
    /// </summary>
    public Status? Status { get; set; }
}
