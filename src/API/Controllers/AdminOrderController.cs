using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TiendaUcnApi.src.Application.DTO;
using TiendaUcnApi.src.Application.DTO.OrderDTO;
using TiendaUcnApi.src.Application.Services.Interfaces;

namespace TiendaUcnApi.src.API.Controllers;

/// <summary>
/// Controlador para la administración de órdenes.
/// Permite ver todas las órdenes, filtrarlas y actualizar su estado.
/// Solo accesible por usuarios con rol "Administrador".
/// </summary>
[ApiController]
[Route("api/admin/orders")]
[Authorize(Roles = "Administrador")]
public class AdminOrderController : ControllerBase
{
    private readonly IOrderService _orderService;

    public AdminOrderController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    /// <summary>
    /// Obtiene todas las órdenes con filtros y paginación.
    /// </summary>
    /// <param name="filter">Filtros de búsqueda.</param>
    /// <returns>Lista paginada de órdenes.</returns>
    [HttpGet]
    public async Task<IActionResult> GetAllOrders([FromQuery] OrderFilterDTO filter)
    {
        var response = await _orderService.GetAllOrdersAsync(filter);
        return Ok(response);
    }

    /// <summary>
    /// Obtiene el detalle de una orden específica por su ID.
    /// </summary>
    /// <param name="id">ID de la orden.</param>
    /// <returns>Detalle de la orden.</returns>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetOrderById(int id)
    {
        var response = await _orderService.GetOrderByIdAsync(id);
        return Ok(response);
    }

    /// <summary>
    /// Actualiza el estado de una orden.
    /// </summary>
    /// <param name="id">ID de la orden.</param>
    /// <param name="dto">DTO con el nuevo estado.</param>
    /// <returns>Orden actualizada.</returns>
    [HttpPatch("{id}/status")]
    public async Task<IActionResult> UpdateOrderStatus(int id, [FromBody] UpdateOrderStatusDTO dto)
    {
        // R125: Extract adminId from authenticated user claims
        var adminIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (adminIdClaim == null || !int.TryParse(adminIdClaim.Value, out var adminId))
        {
            return Unauthorized(new { message = "Invalid admin credentials" });
        }

        var response = await _orderService.UpdateOrderStatusAsync(id, dto, adminId);
        return Ok(response);
    }
}
