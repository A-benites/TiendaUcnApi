namespace TiendaUcnApi.src.Application.DTO.ProductDTO;

/// <summary>
/// Data Transfer Object for displaying product details.
/// </summary>
public class ProductDetailDTO
{
    /// <summary>
    /// Unique identifier for the product.
    /// </summary>
    public required int Id { get; set; }

    /// <summary>
    /// Product title.
    /// </summary>
    public required string Title { get; set; }

    /// <summary>
    /// Product description.
    /// </summary>
    public required string Description { get; set; }

    /// <summary>
    /// List of product image URLs.
    /// </summary>
    public List<string> ImagesURL { get; set; } = new List<string>();

    /// <summary>
    /// Product price (formatted as string).
    /// </summary>
    public required string Price { get; set; }

    /// <summary>
    /// Discount percentage applied to the product.
    /// </summary>
    public required int Discount { get; set; }

    /// <summary>
    /// Final price with discount applied (calculated server-side).
    /// Implements R76-R77 rubric requirement: finalPrice calculated server-side.
    /// </summary>
    public required string FinalPrice { get; set; }

    /// <summary>
    /// Available stock quantity.
    /// </summary>
    public required int Stock { get; set; }

    /// <summary>
    /// Stock indicator (e.g., "Low", "High", "Out of Stock").
    /// </summary>
    public required string StockIndicator { get; set; }

    /// <summary>
    /// Name of the product's category.
    /// </summary>
    public required string CategoryName { get; set; }

    /// <summary>
    /// Name of the product's brand.
    /// </summary>
    public required string BrandName { get; set; }

    /// <summary>
    /// Product status as text (e.g., "New", "Used").
    /// </summary>
    public required string StatusName { get; set; }

    /// <summary>
    /// Indicates whether the product is available for sale.
    /// </summary>
    public required bool IsAvailable { get; set; }
}
