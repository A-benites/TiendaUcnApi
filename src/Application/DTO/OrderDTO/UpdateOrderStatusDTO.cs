using System.ComponentModel.DataAnnotations;
using TiendaUcnApi.src.Domain.Models;

namespace TiendaUcnApi.src.Application.DTO.OrderDTO;

/// <summary>
/// Data Transfer Object for updating an order's status.
/// </summary>
public class UpdateOrderStatusDTO
{
    /// <summary>
    /// New status for the order.
    /// </summary>
    [Required(ErrorMessage = "El estado es requerido.")]
    public OrderStatus Status { get; set; }
}
