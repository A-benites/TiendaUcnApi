using System.ComponentModel.DataAnnotations;
using TiendaUcnApi.src.Domain.Models;

namespace TiendaUcnApi.src.Application.DTO.OrderDTO;

/// <summary>
/// DTO para actualizar el estado de una orden.
/// </summary>
public class UpdateOrderStatusDTO
{
    /// <summary>
    /// Nuevo estado de la orden.
    /// </summary>
    [Required(ErrorMessage = "El estado es requerido.")]
    public OrderStatus Status { get; set; }
}
