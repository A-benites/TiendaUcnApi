namespace TiendaUcnApi.src.Application.DTO.ProductDTO.AdminDTO;

/// <summary>
/// Data Transfer Object for displaying detailed product information in administration.
/// Includes image IDs for management operations.
/// </summary>
public class ProductDetailForAdminDTO
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
    /// List of product images with IDs and URLs for admin management.
    /// </summary>
    public List<ProductImageDTO> Images { get; set; } = new List<ProductImageDTO>();

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
    /// ID of the product's category.
    /// </summary>
    public required int CategoryId { get; set; }

    /// <summary>
    /// Name of the product's brand.
    /// </summary>
    public required string BrandName { get; set; }

    /// <summary>
    /// ID of the product's brand.
    /// </summary>
    public required int BrandId { get; set; }

    /// <summary>
    /// Product status as text (e.g., "New", "Used").
    /// </summary>
    public required string StatusName { get; set; }

    /// <summary>
    /// Indicates whether the product is available for sale.
    /// </summary>
    public required bool IsAvailable { get; set; }

    /// <summary>
    /// Product creation date.
    /// </summary>
    public required DateTime CreatedAt { get; set; }

    /// <summary>
    /// Product last update date.
    /// </summary>
    public required DateTime UpdatedAt { get; set; }
}

/// <summary>
/// DTO for product image with ID and URL.
/// </summary>
public class ProductImageDTO
{
    /// <summary>
    /// Image identifier.
    /// </summary>
    public required int Id { get; set; }

    /// <summary>
    /// Image URL.
    /// </summary>
    public required string Url { get; set; }

    /// <summary>
    /// Cloudinary public ID for the image.
    /// </summary>
    public string? PublicId { get; set; }
}
