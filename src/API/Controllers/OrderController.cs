using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TiendaUcnApi.src.Application.DTO;
using TiendaUcnApi.src.Application.DTO.OrderDTO;
using TiendaUcnApi.src.Application.Services.Interfaces;

namespace TiendaUcnApi.src.API.Controllers;

/// <summary>
/// Controller for managing customer purchase orders.
/// Handles order creation and retrieval for authenticated users.
/// </summary>
[Route("api/orders")]
[ApiController]
[Authorize] // Allow any authenticated user
public class OrderController : BaseController
{
    private readonly IOrderService _orderService;

    /// <summary>
    /// Initializes a new instance of the <see cref="OrderController"/> class.
    /// </summary>
    /// <param name="orderService">The order service for business logic.</param>
    public OrderController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    /// <summary>
    /// Creates a new order from the user's shopping cart.
    /// Converts cart items into an order and clears the cart.
    /// </summary>
    /// <returns>The created order with order items and totals.</returns>
    /// <response code="200">Order created successfully.</response>
    /// <response code="400">Cart is empty or contains invalid items.</response>
    /// <response code="401">User not authenticated.</response>
    [HttpPost]
    public async Task<IActionResult> CreateOrder()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
        {
            return Unauthorized(new GenericResponse<object>("User not authenticated", null));
        }

        // For authenticated users, try to get buyerId from cookie/middleware,
        // but if not available, use userId as buyerId (common for users who never shopped anonymously)
        var buyerId = HttpContext.Items["BuyerId"]?.ToString();
        if (string.IsNullOrEmpty(buyerId))
        {
            buyerId = $"user-{userId}"; // Fallback for authenticated users without cookie
        }

        var response = await _orderService.CreateAsync(buyerId, userId);
        return Ok(response);
    }

    /// <summary>
    /// Retrieves all orders for the authenticated user with pagination.
    /// </summary>
    /// <param name="page">Page number (default 1).</param>
    /// <param name="pageSize">Page size (default 10).</param>
    /// <param name="code">Optional order code to filter by.</param>
    /// <returns>Paginated list of user orders.</returns>
    /// <response code="200">Returns the paginated order list.</response>
    /// <response code="401">User not authenticated.</response>
    [HttpGet]
    public async Task<IActionResult> GetOrders(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? code = null
    )
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
        {
            return Unauthorized(new GenericResponse<object>("User not authenticated", null));
        }

        var filter = new UserOrderFilterDTO { Page = page, PageSize = pageSize, Code = code };

        var response = await _orderService.GetAllByUserPaginated(userId, filter);
        return Ok(response);
    }

    /// <summary>
    /// Retrieves detailed information for a specific order belonging to the authenticated user.
    /// </summary>
    /// <param name="id">The order identifier.</param>
    /// <returns>Order details including all items and pricing.</returns>
    /// <response code="200">Returns the order details.</response>
    /// <response code="401">User not authenticated.</response>
    /// <response code="404">Order not found or doesn't belong to user.</response>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetOrderById(int id)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
        {
            return Unauthorized(new GenericResponse<object>("User not authenticated", null));
        }

        var response = await _orderService.GetOrderDetailByIdAsync(id, userId);
        return Ok(response);
    }

    /// <summary>
    /// Retrieves the buyer identifier from HTTP context.
    /// </summary>
    /// <returns>Buyer identifier string.</returns>
    /// <exception cref="Exception">Thrown when buyer ID is not found.</exception>
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
