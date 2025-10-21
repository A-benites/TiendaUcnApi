using TiendaUcnApi.src.Domain.Models;

namespace TiendaUcnApi.src.Application.DTO.OrderDTO;

/// <summary>
/// DTO para filtrar órdenes.
/// </summary>
public class OrderFilterDTO
{
    /// <summary>
    /// Filtrar por estado de la orden.
    /// </summary>
    public OrderStatus? Status { get; set; }

    /// <summary>
    /// Filtrar por ID de usuario (solo para admin).
    /// </summary>
    public int? UserId { get; set; }

    /// <summary>
    /// Filtrar por fecha de inicio.
    /// </summary>
    public DateTime? StartDate { get; set; }

    /// <summary>
    /// Filtrar por fecha de fin.
    /// </summary>
    public DateTime? EndDate { get; set; }

    /// <summary>
    /// Código de orden para búsqueda.
    /// </summary>
    public string? Code { get; set; }

    /// <summary>
    /// Número de página para paginación.
    /// </summary>
    public int Page { get; set; } = 1;

    /// <summary>
    /// Tamaño de página para paginación.
    /// </summary>
    public int PageSize { get; set; } = 10;
}
