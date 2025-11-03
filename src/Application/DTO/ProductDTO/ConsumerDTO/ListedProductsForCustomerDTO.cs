namespace TiendaUcnApi.src.Application.DTO.ProductDTO.ConsumerDTO;

/// <summary>
/// DTO for displaying summarized product information to customers in lists.
/// </summary>
public class ListedProductsForCustomerDTO
{
    /// <summary>
    /// Unique product identifier.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Product title.
    /// </summary>
    public required string Title { get; set; }

    /// <summary>
    /// Product description.
    /// </summary>
    public required string Description { get; set; }

    /// <summary>
    /// Main product image URL.
    /// </summary>
    public string? MainImageURL { get; set; }

    /// <summary>
    /// Original product price.
    /// </summary>
    public decimal Price { get; set; }

    /// <summary>
    /// Discount percentage applied.
    /// </summary>
    public int Discount { get; set; }

    /// <summary>
    /// Final price with discount applied (calculated server-side).
    /// Implements R71-R72 rubric requirement: finalPrice calculated server-side.
    /// </summary>
    public decimal FinalPrice { get; set; }

    /// <summary>
    /// Available product stock.
    /// </summary>
    public int Stock { get; set; }

    /// <summary>
    /// Product category name.
    /// </summary>
    public string? CategoryName { get; set; }

    /// <summary>
    /// Product brand name.
    /// </summary>
    public string? BrandName { get; set; }
}
