namespace TiendaUcnApi.src.Application.DTO.OrderDTO;

/// <summary>
/// DTO para filtros y paginación de órdenes del usuario.
/// </summary>
public class UserOrderFilterDTO
{
    /// <summary>
    /// Número de página (inicia en 1).
    /// </summary>
    public int Page { get; set; } = 1;

    /// <summary>
    /// Tamaño de página (cantidad de órdenes por página).
    /// </summary>
    public int PageSize { get; set; } = 10;
}
