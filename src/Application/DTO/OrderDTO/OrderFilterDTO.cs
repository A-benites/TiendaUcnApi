using TiendaUcnApi.src.Domain.Models;

namespace TiendaUcnApi.src.Application.DTO.OrderDTO;

/// <summary>
/// Sorting options for orders.
/// Implements R116 rubric requirement: safe ordering by createdAt and total.
/// </summary>
public enum OrderSortOption
{
    /// <summary>
    /// Sort by creation date descending (most recent first).
    /// </summary>
    CreatedAtDesc,

    /// <summary>
    /// Sort by creation date ascending (oldest first).
    /// </summary>
    CreatedAtAsc,

    /// <summary>
    /// Sort by total amount descending (highest to lowest).
    /// </summary>
    TotalDesc,

    /// <summary>
    /// Sort by total amount ascending (lowest to highest).
    /// </summary>
    TotalAsc,
}

/// <summary>
/// Data Transfer Object for filtering orders.
/// </summary>
public class OrderFilterDTO
{
    /// <summary>
    /// Filter by order status.
    /// </summary>
    public OrderStatus? Status { get; set; }

    /// <summary>
    /// Filter by user ID (admin only).
    /// </summary>
    public int? UserId { get; set; }

    /// <summary>
    /// Filter by start date.
    /// </summary>
    public DateTime? StartDate { get; set; }

    /// <summary>
    /// Filter by end date.
    /// </summary>
    public DateTime? EndDate { get; set; }

    /// <summary>
    /// Order code for search.
    /// </summary>
    public string? Code { get; set; }

    /// <summary>
    /// Sorting option for orders.
    /// Implements R116 rubric requirement.
    /// </summary>
    public OrderSortOption? SortBy { get; set; }

    /// <summary>
    /// Page number for pagination.
    /// </summary>
    public int Page { get; set; } = 1;

    /// <summary>
    /// Page size for pagination.
    /// </summary>
    public int PageSize { get; set; } = 10;
}
