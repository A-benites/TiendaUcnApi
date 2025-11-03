using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TiendaUcnApi.src.Application.DTO;
using TiendaUcnApi.src.Application.DTO.CartDTO;
using TiendaUcnApi.src.Application.Services.Interfaces;

namespace TiendaUcnApi.src.API.Controllers
{
    /// <summary>
    /// Controller for managing shopping cart operations.
    /// Supports both anonymous (cookie-based) and authenticated user carts.
    /// </summary>
    public class CartController : BaseController
    {
        private readonly ICartService _cartService;

        /// <summary>
        /// Initializes a new instance of the <see cref="CartController"/> class.
        /// </summary>
        /// <param name="cartService">The cart service for business logic.</param>
        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }

        /// <summary>
        /// Retrieves the current user's shopping cart.
        /// Creates a new cart if one doesn't exist. Generates a buyerId cookie for anonymous users.
        /// </summary>
        /// <returns>Shopping cart with all items and pricing.</returns>
        /// <response code="200">Returns the shopping cart.</response>
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetCart()
        {
            string? userId =
                User.Identity?.IsAuthenticated == true
                    ? User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value
                    : null;

            int? parsedUserId = userId != null && int.TryParse(userId, out int id) ? id : null;

            string? buyerId = Request.Cookies["buyerId"];

            if (string.IsNullOrEmpty(buyerId))
            {
                buyerId = Guid.NewGuid().ToString();

                var cookieOptions = new CookieOptions
                {
                    IsEssential = true,
                    Expires = DateTime.UtcNow.AddDays(30),
                    SameSite = SameSiteMode.None,
                    Secure = true,
                };

                Response.Cookies.Append("buyerId", buyerId, cookieOptions);
            }

            var cart = await _cartService.CreateOrGetAsync(buyerId, parsedUserId);

            return Ok(new GenericResponse<CartDTO>("Cart retrieved successfully", cart));
        }

        /// <summary>
        /// Adds a product to the shopping cart or updates quantity if already present.
        /// </summary>
        /// <param name="addCartItemDTO">Product ID and quantity to add.</param>
        /// <returns>Updated shopping cart.</returns>
        /// <response code="200">Item added successfully.</response>
        /// <response code="400">Invalid product ID or insufficient stock.</response>
        [HttpPost("items")]
        [AllowAnonymous]
        public async Task<IActionResult> AddItem([FromBody] AddCartItemDTO addCartItemDTO)
        {
            var buyerId = GetBuyerId();
            var userId =
                User.Identity?.IsAuthenticated == true
                    ? User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value
                    : null;
            var parsedUserId = userId != null && int.TryParse(userId, out int id) ? id : (int?)null;
            var result = await _cartService.AddItemAsync(
                buyerId,
                addCartItemDTO.ProductId,
                addCartItemDTO.Quantity,
                parsedUserId
            );
            return Ok(new GenericResponse<CartDTO>("Item added successfully", result));
        }

        /// <summary>
        /// Removes a product from the shopping cart.
        /// </summary>
        /// <param name="productId">The product identifier to remove.</param>
        /// <returns>Updated shopping cart.</returns>
        /// <response code="200">Item removed successfully.</response>
        /// <response code="404">Item not found in cart.</response>
        [HttpDelete("items/{productId}")]
        [AllowAnonymous]
        public async Task<IActionResult> RemoveItem(int productId)
        {
            var buyerId = GetBuyerId();
            var userId =
                User.Identity?.IsAuthenticated == true
                    ? User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value
                    : null;
            var parsedUserId = userId != null && int.TryParse(userId, out int id) ? id : (int?)null;
            var result = await _cartService.RemoveItemAsync(buyerId, productId, parsedUserId);
            return Ok(new GenericResponse<CartDTO>("Item removed successfully", result));
        }

        /// <summary>
        /// Clears all items from the shopping cart.
        /// </summary>
        /// <returns>Empty shopping cart.</returns>
        /// <response code="200">Cart cleared successfully.</response>
        [HttpPost("clear")]
        [AllowAnonymous]
        public async Task<IActionResult> ClearCart()
        {
            var buyerId = GetBuyerId();
            var userId =
                User.Identity?.IsAuthenticated == true
                    ? User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value
                    : null;
            var parsedUserId = userId != null && int.TryParse(userId, out int id) ? id : (int?)null;
            var result = await _cartService.ClearAsync(buyerId, parsedUserId);
            return Ok(new GenericResponse<CartDTO>("Cart cleared successfully", result));
        }

        /// <summary>
        /// Updates the quantity of a specific item in the cart.
        /// </summary>
        /// <param name="changeItemQuantityDTO">Product ID and new quantity.</param>
        /// <returns>Updated shopping cart.</returns>
        /// <response code="200">Item quantity updated successfully.</response>
        /// <response code="400">Invalid quantity or insufficient stock.</response>
        [HttpPatch("items")]
        [AllowAnonymous]
        public async Task<IActionResult> UpdateItemQuantity(
            [FromBody] ChangeItemQuantityDTO changeItemQuantityDTO
        )
        {
            var buyerId = GetBuyerId();
            var userId =
                User.Identity?.IsAuthenticated == true
                    ? User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value
                    : null;
            var parsedUserId = userId != null && int.TryParse(userId, out int id) ? id : (int?)null;
            var result = await _cartService.UpdateItemQuantityAsync(
                buyerId,
                changeItemQuantityDTO.ProductId,
                changeItemQuantityDTO.Quantity,
                parsedUserId
            );
            return Ok(new GenericResponse<CartDTO>("Item quantity updated successfully", result));
        }

        /// <summary>
        /// Validates the cart for checkout (authenticated users only).
        /// Checks stock availability and ensures all items are valid.
        /// </summary>
        /// <returns>Validated shopping cart ready for order creation.</returns>
        /// <response code="200">Checkout validation successful.</response>
        /// <response code="401">User not authenticated.</response>
        /// <response code="400">Cart is empty or contains invalid items.</response>
        [HttpPost("checkout")]
        [Authorize(Roles = "Cliente")]
        public async Task<IActionResult> CheckoutAsync()
        {
            var buyerId = GetBuyerId();
            var userId =
                User.Identity?.IsAuthenticated == true
                    ? User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value
                    : null;
            var parsedUserId = userId != null && int.TryParse(userId, out int id) ? id : (int?)null;
            var result = await _cartService.CheckoutAsync(buyerId, parsedUserId);
            return Ok(new GenericResponse<CartDTO>("Checkout completed successfully", result));
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
}
