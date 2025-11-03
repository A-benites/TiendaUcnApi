namespace TiendaUcnApi.src.Application.DTO.ProductDTO.AdminDTO;

/// <summary>
/// DTO representing a paginated list of products for administration.
/// </summary>
public class ListedProductsForAdminDTO
{
    /// <summary>
    /// List of products for administration.
    /// </summary>
    public List<ProductForAdminDTO> Products { get; set; } = new List<ProductForAdminDTO>();

    /// <summary>
    /// Total count of products found.
    /// </summary>
    public int TotalCount { get; set; }

    /// <summary>
    /// Total number of pages available.
    /// </summary>
    public int TotalPages { get; set; }

    /// <summary>
    /// Current query page.
    /// </summary>
    public int CurrentPage { get; set; }

    /// <summary>
    /// Number of products per page.
    /// </summary>
    public int PageSize { get; set; }
}
