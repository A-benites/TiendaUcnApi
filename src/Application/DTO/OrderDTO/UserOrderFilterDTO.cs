namespace TiendaUcnApi.src.Application.DTO.OrderDTO;

/// <summary>
/// Data Transfer Object for filtering and paginating user orders.
/// </summary>
public class UserOrderFilterDTO
{
    /// <summary>
    /// Page number (starts at 1).
    /// </summary>
    public int Page { get; set; } = 1;

    /// <summary>
    /// Page size (number of orders per page).
    /// </summary>
    public int PageSize { get; set; } = 10;
}
