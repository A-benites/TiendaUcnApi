using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TiendaUcnApi.src.Application.DTO;
using TiendaUcnApi.src.Application.DTO.OrderDTO;
using TiendaUcnApi.src.Application.Services.Interfaces;

namespace TiendaUcnApi.src.API.Controllers;

/// <summary>
/// Controller for administrative order management.
/// Allows viewing all orders, filtering them, and updating their status.
/// Only accessible by users with "Administrador" role.
/// </summary>
[ApiController]
[Route("api/admin/orders")]
[Authorize(Roles = "Administrador")]
public class AdminOrderController : ControllerBase
{
    private readonly IOrderService _orderService;

    /// <summary>
    /// Initializes a new instance of the <see cref="AdminOrderController"/> class.
    /// </summary>
    /// <param name="orderService">The order service for business logic.</param>
    public AdminOrderController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    /// <summary>
    /// Retrieves all orders with filters and pagination.
    /// </summary>
    /// <param name="filter">Search filters.</param>
    /// <returns>Paginated list of orders.</returns>
    /// <response code="200">Returns the order list.</response>
    [HttpGet]
    public async Task<IActionResult> GetAllOrders([FromQuery] OrderFilterDTO filter)
    {
        var response = await _orderService.GetAllOrdersAsync(filter);
        return Ok(response);
    }

    /// <summary>
    /// Retrieves the details of a specific order by its ID.
    /// </summary>
    /// <param name="id">Order ID.</param>
    /// <returns>Order details.</returns>
    /// <response code="200">Returns the order details.</response>
    /// <response code="404">Order not found.</response>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetOrderById(int id)
    {
        var response = await _orderService.GetOrderByIdAsync(id);
        return Ok(response);
    }

    /// <summary>
    /// Updates the status of an order.
    /// Extracts the admin ID from authenticated user claims to track who performed the status change.
    /// </summary>
    /// <param name="id">Order ID.</param>
    /// <param name="dto">DTO with the new status.</param>
    /// <returns>Updated order.</returns>
    /// <response code="200">Order updated successfully.</response>
    /// <response code="401">Invalid admin credentials.</response>
    /// <response code="404">Order not found.</response>
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
