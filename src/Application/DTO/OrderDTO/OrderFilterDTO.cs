using TiendaUcnApi.src.Domain.Models;

namespace TiendaUcnApi.src.Application.DTO.OrderDTO;

/// <summary>
/// Opciones de ordenamiento para órdenes.
/// Implements R116 rubric requirement: safe ordering by createdAt and total.
/// </summary>
public enum OrderSortOption
{
    /// <summary>
    /// Ordenar por fecha de creación descendente (más recientes primero).
    /// </summary>
    CreatedAtDesc,

    /// <summary>
    /// Ordenar por fecha de creación ascendente (más antiguos primero).
    /// </summary>
    CreatedAtAsc,

    /// <summary>
    /// Ordenar por total descendente (mayor a menor).
    /// </summary>
    TotalDesc,

    /// <summary>
    /// Ordenar por total ascendente (menor a mayor).
    /// </summary>
    TotalAsc,
}

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
    /// Opción de ordenamiento para las órdenes.
    /// Implements R116 rubric requirement.
    /// </summary>
    public OrderSortOption? SortBy { get; set; }

    /// <summary>
    /// Número de página para paginación.
    /// </summary>
    public int Page { get; set; } = 1;

    /// <summary>
    /// Tamaño de página para paginación.
    /// </summary>
    public int PageSize { get; set; } = 10;
}
