namespace TiendaUcnApi.src.Application.DTO.ProductDTO.AdminDTO;

/// <summary>
/// DTO for displaying product information in administration.
/// </summary>
public class ProductForAdminDTO
{
    /// <summary>
    /// Unique product identifier.
    /// </summary>
    public required int Id { get; set; }

    /// <summary>
    /// Product title.
    /// </summary>
    public required string Title { get; set; }

    /// <summary>
    /// Main product image URL.
    /// </summary>
    public string? MainImageURL { get; set; }

    /// <summary>
    /// Product price.
    /// </summary>
    public required string Price { get; set; }

    /// <summary>
    /// Available product stock.
    /// </summary>
    public required int Stock { get; set; }

    /// <summary>
    /// Stock indicator (e.g., "Low", "High").
    /// </summary>
    public required string StockIndicator { get; set; }

    /// <summary>
    /// Product category name.
    /// </summary>
    public required string CategoryName { get; set; }

    /// <summary>
    /// Product brand name.
    /// </summary>
    public required string BrandName { get; set; }

    /// <summary>
    /// Product status text.
    /// </summary>
    public required string StatusName { get; set; }

    /// <summary>
    /// Indicates whether the product is available for sale.
    /// </summary>
    public required bool IsAvailable { get; set; }

    /// <summary>
    /// Product last update date.
    /// </summary>
    public required DateTime UpdatedAt { get; set; }
}
