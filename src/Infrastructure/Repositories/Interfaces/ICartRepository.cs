using TiendaUcnApi.src.Domain.Models;

namespace TiendaUcnApi.src.Infrastructure.Repositories.Interfaces
{
    /// <summary>
    /// Repository interface for shopping cart data access operations.
    /// Handles both anonymous (cookie-based) and authenticated user carts.
    /// </summary>
    public interface ICartRepository
    {
        /// <summary>
        /// Finds a cart by buyer ID or user ID.
        /// </summary>
        /// <param name="buyerId">Anonymous buyer identifier from cookie.</param>
        /// <param name="userId">Authenticated user identifier (optional).</param>
        /// <returns>Cart entity or null if not found.</returns>
        Task<Cart?> FindAsync(string buyerId, int? userId);

        /// <summary>
        /// Retrieves a cart by authenticated user ID.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns>Cart entity or null if not found.</returns>
        Task<Cart?> GetByUserIdAsync(int userId);

        /// <summary>
        /// Retrieves a cart for an anonymous user by buyer ID.
        /// </summary>
        /// <param name="buyerId">Anonymous buyer identifier from cookie.</param>
        /// <returns>Cart entity or null if not found.</returns>
        Task<Cart?> GetAnonymousAsync(string buyerId);

        /// <summary>
        /// Retrieves a cart by buyer ID (works for both anonymous and authenticated).
        /// </summary>
        /// <param name="buyerId">Buyer identifier.</param>
        /// <returns>Cart entity or null if not found.</returns>
        Task<Cart?> GetByBuyerIdAsync(string buyerId);

        /// <summary>
        /// Creates a new shopping cart.
        /// </summary>
        /// <param name="buyerId">Anonymous buyer identifier.</param>
        /// <param name="userId">Optional authenticated user identifier.</param>
        /// <returns>The created cart entity.</returns>
        Task<Cart> CreateAsync(string buyerId, int? userId = null);

        /// <summary>
        /// Updates an existing cart in the database.
        /// </summary>
        /// <param name="cart">The cart entity with updated data.</param>
        Task UpdateAsync(Cart cart);

        /// <summary>
        /// Deletes a cart from the database.
        /// </summary>
        /// <param name="cart">The cart entity to delete.</param>
        Task DeleteAsync(Cart cart);

        /// <summary>
        /// Adds an item to a shopping cart.
        /// </summary>
        /// <param name="cart">The cart to add the item to.</param>
        /// <param name="cartItem">The cart item to add.</param>
        Task AddItemAsync(Cart cart, CartItem cartItem);

        /// <summary>
        /// Removes an item from a shopping cart.
        /// </summary>
        /// <param name="cartItem">The cart item to remove.</param>
        Task RemoveItemAsync(CartItem cartItem);

        /// <summary>
        /// Retrieves all carts including user, items, and products (for admin/background jobs).
        /// </summary>
        /// <returns>List of all carts with related entities.</returns>
        Task<List<Cart>> GetAllAsync();

        /// <summary>
        /// Retrieves abandoned carts (not updated in the last 3 days).
        /// Used by background jobs to send reminder emails.
        /// </summary>
        /// <returns>List of abandoned carts.</returns>
        Task<List<Cart>> GetAbandonedCartsAsync();
    }
}
