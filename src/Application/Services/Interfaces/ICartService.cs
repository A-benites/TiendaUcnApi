using TiendaUcnApi.src.Application.DTO.CartDTO;

namespace TiendaUcnApi.src.Application.Services.Interfaces
{
    /// <summary>
    /// Interface for managing shopping cart operations.
    /// </summary>
    public interface ICartService
    {
        /// <summary>
        /// Adds a product to the cart or increases its quantity.
        /// </summary>
        Task<CartDTO> AddItemAsync(string buyerId, int productId, int quantity, int? userId = null);

        /// <summary>
        /// Creates or retrieves an existing cart.
        /// </summary>
        Task<CartDTO> CreateOrGetAsync(string buyerId, int? userId = null);

        /// <summary>
        /// Removes an item from the cart.
        /// </summary>
        Task<CartDTO> RemoveItemAsync(string buyerId, int productId, int? userId = null);

        /// <summary>
        /// Clears all items from the cart.
        /// </summary>
        Task<CartDTO> ClearAsync(string buyerId, int? userId = null);

        /// <summary>
        /// Updates the quantity of an item in the cart.
        /// </summary>
        Task<CartDTO> UpdateItemQuantityAsync(string buyerId, int productId, int quantity, int? userId = null);

        /// <summary>
        /// Associates an anonymous cart with a logged-in user.
        /// </summary>
        Task AssociateWithUserAsync(string buyerId, int userId);

        /// <summary>
        /// Validates stock and finalizes the cart checkout process.
        /// </summary>
        Task<CartDTO> CheckoutAsync(string buyerId, int? userId);

        /// <summary>
        /// Retrieves a list of carts that have been inactive for a specific number of days.
        /// This is used by Hangfire to send abandoned cart reminders.
        /// </summary>
        Task<IEnumerable<CartDTO>> GetAbandonedCartsAsync(int daysInactive);
    }
}
