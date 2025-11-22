using System.Globalization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using TiendaUcnApi.src.Domain.Models;
using TiendaUcnApi.src.Infrastructure.Data;
using TiendaUcnApi.src.Infrastructure.Repositories.Interfaces;

namespace TiendaUcnApi.src.Infrastructure.Repositories.Implements
{
    /// <summary>
    /// Implementation of the shopping cart repository.
    /// Handles database operations for shopping carts, supporting both authenticated and anonymous users.
    /// </summary>
    public class CartRepository : ICartRepository
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;

        /// <summary>
        /// Initializes a new instance of the <see cref="CartRepository"/> class.
        /// </summary>
        /// <param name="dataContext">Database context.</param>
        /// <param name="configuration">Application configuration.</param>
        public CartRepository(AppDbContext dataContext, IConfiguration configuration)
        {
            _context = dataContext;
            _configuration = configuration;
        }

        /// <summary>
        /// Finds a cart by buyer ID and optionally associates it with a user.
        /// Handles cart migration from anonymous to authenticated users.
        /// </summary>
        /// <param name="buyerId">The buyer identifier (cookie-based).</param>
        /// <param name="userId">Optional user ID for authenticated users.</param>
        /// <returns>The found or migrated cart, or null if not found.</returns>
        public async Task<Cart?> FindAsync(string buyerId, int? userId)
        {
            Cart? cart = null;

            if (userId.HasValue)
            {
                cart = await _context
                    .Carts.Include(c => c.CartItems)
                    .ThenInclude(ci => ci.Product)
                    .ThenInclude(p => p.Images)
                    .FirstOrDefaultAsync(c => c.UserId == userId);

                if (cart != null)
                {
                    if (cart.BuyerId != buyerId)
                    {
                        cart.BuyerId = buyerId;
                        cart.UpdatedAt = DateTime.UtcNow;
                        await _context.SaveChangesAsync();
                    }
                    return cart;
                }
            }

            cart = await _context
                .Carts.Include(c => c.CartItems)
                .ThenInclude(ci => ci.Product)
                .ThenInclude(p => p.Images)
                .FirstOrDefaultAsync(c => c.BuyerId == buyerId && c.UserId == null);

            if (cart != null && userId.HasValue)
            {
                cart.UserId = userId;
                cart.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }

            return cart;
        }

        /// <summary>
        /// Retrieves a cart by user ID.
        /// </summary>
        /// <param name="userId">The user ID.</param>
        /// <returns>The user's cart, or null if not found.</returns>
        public async Task<Cart?> GetByUserIdAsync(int userId)
        {
            return await _context
                .Carts.Include(c => c.CartItems)
                .ThenInclude(ci => ci.Product)
                .ThenInclude(p => p.Images)
                .FirstOrDefaultAsync(c => c.UserId == userId);
        }

        /// <summary>
        /// Retrieves an anonymous cart by buyer ID.
        /// </summary>
        /// <param name="buyerId">The buyer identifier (cookie-based).</param>
        /// <returns>The anonymous cart, or null if not found.</returns>
        public async Task<Cart?> GetAnonymousAsync(string buyerId)
        {
            return await _context
                .Carts.Include(c => c.CartItems)
                .ThenInclude(ci => ci.Product)
                .ThenInclude(p => p.Images)
                .FirstOrDefaultAsync(c => c.BuyerId == buyerId && c.UserId == null);
        }

        /// <summary>
        /// Creates a new cart for a buyer.
        /// </summary>
        /// <param name="buyerId">The buyer identifier.</param>
        /// <param name="userId">Optional user ID for authenticated users.</param>
        /// <returns>The created cart with all related data.</returns>
        public async Task<Cart> CreateAsync(string buyerId, int? userId = null)
        {
            var cart = new Cart
            {
                BuyerId = buyerId,
                UserId = userId,
                SubTotal = 0,
                Total = 0,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
            };

            _context.Carts.Add(cart);
            await _context.SaveChangesAsync();

            return await _context
                .Carts.Include(c => c.CartItems)
                .ThenInclude(ci => ci.Product)
                .ThenInclude(p => p.Images)
                .FirstAsync(c => c.Id == cart.Id);
        }

        /// <summary>
        /// Updates an existing cart.
        /// </summary>
        /// <param name="cart">The cart to update.</param>
        public async Task UpdateAsync(Cart cart)
        {
            cart.UpdatedAt = DateTime.UtcNow;
            _context.Carts.Update(cart);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Deletes a cart.
        /// </summary>
        /// <param name="cart">The cart to delete.</param>
        public async Task DeleteAsync(Cart cart)
        {
            _context.Carts.Remove(cart);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Adds an item to a cart.
        /// </summary>
        /// <param name="cart">The cart to add the item to.</param>
        /// <param name="cartItem">The item to add.</param>
        public async Task AddItemAsync(Cart cart, CartItem cartItem)
        {
            _context.Attach(cartItem.Product);

            if (cartItem.Product.Brand != null)
                _context.Attach(cartItem.Product.Brand);
            if (cartItem.Product.Category != null)
                _context.Attach(cartItem.Product.Category);

            _context.Attach(cart);
            _context.CartItems.Add(cartItem);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Removes an item from a cart.
        /// </summary>
        /// <param name="cartItem">The cart item to remove.</param>
        public async Task RemoveItemAsync(CartItem cartItem)
        {
            _context.CartItems.Remove(cartItem);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Retrieves a cart by buyer ID with all related entities.
        /// </summary>
        /// <param name="buyerId">The buyer identifier.</param>
        /// <returns>The cart with all items, products, brands, categories, and images.</returns>
        public async Task<Cart?> GetByBuyerIdAsync(string buyerId)
        {
            return await _context
                .Carts.Include(c => c.CartItems)
                .ThenInclude(ci => ci.Product)
                .ThenInclude(p => p.Images)
                .Include(c => c.CartItems)
                .ThenInclude(ci => ci.Product)
                .ThenInclude(p => p.Brand)
                .Include(c => c.CartItems)
                .ThenInclude(ci => ci.Product)
                .ThenInclude(p => p.Category)
                .FirstOrDefaultAsync(c => c.BuyerId == buyerId);
        }

        /// <summary>
        /// Retrieves all carts (for debugging or maintenance purposes).
        /// </summary>
        /// <returns>List of all carts with related data.</returns>
        public async Task<List<Cart>> GetAllAsync()
        {
            return await _context
                .Carts.Include(c => c.User)
                .Include(c => c.CartItems)
                .ThenInclude(i => i.Product)
                .ThenInclude(p => p.Images)
                .ToListAsync();
        }

        /// <summary>
        /// Retrieves abandoned carts based on configuration threshold.
        /// Configurable in appsettings.json and excludes seed users.
        /// </summary>
        /// <returns>List of abandoned carts eligible for reminder emails.</returns>
        public async Task<List<Cart>> GetAbandonedCartsAsync()
        {
            // Read abandoned days from appsettings.json (default: 3)
            var abandonedDays = _configuration.GetValue<int>("Cart:AbandonedCartDays", 3);
            DateTime threshold = DateTime.UtcNow.AddDays(-abandonedDays);

            return await _context
                .Carts.Include(c => c.User)
                .Include(c => c.CartItems)
                .ThenInclude(i => i.Product)
                .ThenInclude(p => p.Images)
                .Where(c =>
                    c.UpdatedAt < threshold
                    && c.CartItems.Any()
                    && c.User != null
                    && c.User.Email != null
                    && c.User.IsSeed == false // Exclude seed users
                )
                .ToListAsync();
        }
    }
}
