namespace TiendaUcnApi.src.Application.DTO.ProductDTO.ConsumerDTO;

/// <summary>
/// DTO for product listing response with pagination metadata.
/// Implements R67 rubric requirement: complete pagination metadata.
/// </summary>
public class ProductListResponseDTO
{
    /// <summary>
    /// List of products.
    /// </summary>
    public List<ListedProductsForCustomerDTO> Products { get; set; } =
        new List<ListedProductsForCustomerDTO>();

    /// <summary>
    /// Total count of products matching the filters.
    /// </summary>
    public int TotalCount { get; set; }

    /// <summary>
    /// Current page number.
    /// </summary>
    public int Page { get; set; }

    /// <summary>
    /// Page size (number of products per page).
    /// </summary>
    public int PageSize { get; set; }

    /// <summary>
    /// Total number of pages available.
    /// </summary>
    public int TotalPages { get; set; }
}
