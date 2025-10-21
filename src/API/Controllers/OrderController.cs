using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TiendaUcnApi.src.Application.DTO;
using TiendaUcnApi.src.Application.Services.Interfaces;

namespace TiendaUcnApi.src.API.Controllers;

/// <summary>
/// Controlador para gestionar órdenes de compra.
/// </summary>
[Route("api/orders")]
[ApiController]
[Authorize(Roles = "Customer")]
public class OrderController : BaseController
{
    private readonly IOrderService _orderService;

    public OrderController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    /// <summary>
    /// Crea una nueva orden a partir del carrito del usuario.
    /// </summary>
    /// <returns>Orden creada.</returns>
    [HttpPost]
    public async Task<IActionResult> CreateOrder()
    {
        var buyerId = GetBuyerId();
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
        {
            return Unauthorized(new GenericResponse<object>("Usuario no autenticado", null));
        }

        var response = await _orderService.CreateAsync(buyerId, userId);
        return Ok(response);
    }

    /// <summary>
    /// Obtiene todas las órdenes del usuario autenticado.
    /// </summary>
    /// <returns>Lista de órdenes.</returns>
    [HttpGet]
    public async Task<IActionResult> GetOrders()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
        {
            return Unauthorized(new GenericResponse<object>("Usuario no autenticado", null));
        }

        var response = await _orderService.GetAllByUser(userId);
        return Ok(response);
    }

    private string GetBuyerId()
    {
        var buyerId = HttpContext.Items["BuyerId"]?.ToString();

        if (string.IsNullOrEmpty(buyerId))
        {
            throw new Exception("Buyer ID not found.");
        }
        return buyerId;
    }
}
