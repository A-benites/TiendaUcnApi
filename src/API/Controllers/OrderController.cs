using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TiendaUcnApi.src.Application.DTO;
using TiendaUcnApi.src.Application.DTO.OrderDTO;
using TiendaUcnApi.src.Application.Services.Interfaces;

namespace TiendaUcnApi.src.API.Controllers;

/// <summary>
/// Controlador para gestionar órdenes de compra.
/// </summary>
[Route("api/orders")]
[ApiController]
[Authorize] // Permitir cualquier usuario autenticado
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
    /// Obtiene todas las órdenes del usuario autenticado con paginación.
    /// </summary>
    /// <param name="page">Número de página (por defecto 1).</param>
    /// <param name="pageSize">Tamaño de página (por defecto 10).</param>
    /// <returns>Lista paginada de órdenes.</returns>
    [HttpGet]
    public async Task<IActionResult> GetOrders([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
        {
            return Unauthorized(new GenericResponse<object>("Usuario no autenticado", null));
        }

        var filter = new UserOrderFilterDTO
        {
            Page = page,
            PageSize = pageSize
        };

        var response = await _orderService.GetAllByUserPaginated(userId, filter);
        return Ok(response);
    }

    /// <summary>
    /// Obtiene el detalle de una orden específica del usuario autenticado.
    /// </summary>
    /// <param name="id">ID de la orden.</param>
    /// <returns>Detalle de la orden.</returns>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetOrderById(int id)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
        {
            return Unauthorized(new GenericResponse<object>("Usuario no autenticado", null));
        }

        var response = await _orderService.GetOrderDetailByIdAsync(id, userId);
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
