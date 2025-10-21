namespace TiendaUcnApi.src.Application.DTO.OrderDTO;

/// <summary>
/// DTO para representar una lista paginada de órdenes.
/// </summary>
public class OrderListDTO
{
    /// <summary>
    /// Lista de órdenes.
    /// </summary>
    public List<OrderDTO> Orders { get; set; } = new List<OrderDTO>();

    /// <summary>
    /// Total de órdenes.
    /// </summary>
    public int TotalCount { get; set; }

    /// <summary>
    /// Página actual.
    /// </summary>
    public int CurrentPage { get; set; }

    /// <summary>
    /// Tamaño de página.
    /// </summary>
    public int PageSize { get; set; }

    /// <summary>
    /// Total de páginas.
    /// </summary>
    public int TotalPages { get; set; }
}
