namespace TiendaUcnApi.src.Application.DTO.OrderDTO;

/// <summary>
/// Data Transfer Object representing a paginated list of orders.
/// </summary>
public class OrderListDTO
{
    /// <summary>
    /// List of orders in the current page.
    /// </summary>
    public List<OrderDTO> Orders { get; set; } = new List<OrderDTO>();

    /// <summary>
    /// Total count of orders matching the filter criteria.
    /// </summary>
    public int TotalCount { get; set; }

    /// <summary>
    /// Current page number.
    /// </summary>
    public int CurrentPage { get; set; }

    /// <summary>
    /// Number of items per page.
    /// </summary>
    public int PageSize { get; set; }

    /// <summary>
    /// Total number of pages available.
    /// </summary>
    public int TotalPages { get; set; }
}
