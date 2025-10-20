        using System.Security.Claims;
        using Microsoft.AspNetCore.Authorization;
        using Microsoft.AspNetCore.Mvc;
        using TiendaUcnApi.src.Application.DTO;
        using TiendaUcnApi.src.Application.DTO.CartDTO;
        using TiendaUcnApi.src.Application.Services.Interfaces;

        namespace TiendaUcnApi.src.API.Controllers
        {
            /// <summary>
            /// Controller for managing the shopping cart.
            /// </summary>
            public class CartController : BaseController
            {
                private readonly ICartService _cartService;

                public CartController(ICartService cartService)
                {
                    _cartService = cartService;
                }

                [HttpGet]
                [AllowAnonymous]
                public async Task<IActionResult> GetCart()
                {
                    
                    string? userId = User.Identity?.IsAuthenticated == true
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
                            Secure = true
                        };

                        Response.Cookies.Append("buyerId", buyerId, cookieOptions);
                    }

                    
                    var cart = await _cartService.CreateOrGetAsync(buyerId, parsedUserId);

                    return Ok(new GenericResponse<CartDTO>("Cart retrieved successfully", cart));
                }


                [HttpPost("items")]
                [AllowAnonymous]
                public async Task<IActionResult> AddItem([FromForm] AddCartItemDTO addCartItemDTO)
                {
                    var buyerId = GetBuyerId();
                    var userId = User.Identity?.IsAuthenticated == true
                        ? User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value
                        : null;
                    var parsedUserId = userId != null && int.TryParse(userId, out int id) ? id : (int?)null;
                    var result = await _cartService.AddItemAsync(buyerId, addCartItemDTO.ProductId, addCartItemDTO.Quantity, parsedUserId);
                    return Ok(new GenericResponse<CartDTO>("Item added successfully", result));
                }

                [HttpDelete("items/{productId}")]
                [AllowAnonymous]
                public async Task<IActionResult> RemoveItem(int productId)
                {
                    var buyerId = GetBuyerId();
                    var userId = User.Identity?.IsAuthenticated == true
                        ? User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value
                        : null;
                    var parsedUserId = userId != null && int.TryParse(userId, out int id) ? id : (int?)null;
                    var result = await _cartService.RemoveItemAsync(buyerId, productId, parsedUserId);
                    return Ok(new GenericResponse<CartDTO>("Item removed successfully", result));
                }

                [HttpPost("clear")]
                [AllowAnonymous]
                public async Task<IActionResult> ClearCart()
                {
                    var buyerId = GetBuyerId();
                    var userId = User.Identity?.IsAuthenticated == true
                        ? User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value
                        : null;
                    var parsedUserId = userId != null && int.TryParse(userId, out int id) ? id : (int?)null;
                    var result = await _cartService.ClearAsync(buyerId, parsedUserId);
                    return Ok(new GenericResponse<CartDTO>("Cart cleared successfully", result));
                }

                [HttpPatch("items")]
                [AllowAnonymous]
                public async Task<IActionResult> UpdateItemQuantity([FromForm] ChangeItemQuantityDTO changeItemQuantityDTO)
                {
                    var buyerId = GetBuyerId();
                    var userId = User.Identity?.IsAuthenticated == true
                        ? User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value
                        : null;
                    var parsedUserId = userId != null && int.TryParse(userId, out int id) ? id : (int?)null;
                    var result = await _cartService.UpdateItemQuantityAsync(buyerId, changeItemQuantityDTO.ProductId, changeItemQuantityDTO.Quantity, parsedUserId);
                    return Ok(new GenericResponse<CartDTO>("Item quantity updated successfully", result));
                }

                [HttpPost("checkout")]
                [Authorize(Roles = "Customer")]
                public async Task<IActionResult> CheckoutAsync()
                {
                    var buyerId = GetBuyerId();
                    var userId = User.Identity?.IsAuthenticated == true
                        ? User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value
                        : null;
                    var parsedUserId = userId != null && int.TryParse(userId, out int id) ? id : (int?)null;
                    var result = await _cartService.CheckoutAsync(buyerId, parsedUserId);
                    return Ok(new GenericResponse<CartDTO>("Checkout completed successfully", result));
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
        }
