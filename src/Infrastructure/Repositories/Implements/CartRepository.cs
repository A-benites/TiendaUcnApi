using System.Globalization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using TiendaUcnApi.src.Domain.Models;
using TiendaUcnApi.src.Infrastructure.Data;
using TiendaUcnApi.src.Infrastructure.Repositories.Interfaces;

namespace TiendaUcnApi.src.Infrastructure.Repositories.Implements
{
    public class CartRepository : ICartRepository
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;

        public CartRepository(AppDbContext dataContext, IConfiguration configuration)
        {
            _context = dataContext;
            _configuration = configuration;
        }

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

        public async Task<Cart?> GetByUserIdAsync(int userId)
        {
            return await _context
                .Carts.Include(c => c.CartItems)
                .ThenInclude(ci => ci.Product)
                .FirstOrDefaultAsync(c => c.UserId == userId);
        }

        public async Task<Cart?> GetAnonymousAsync(string buyerId)
        {
            return await _context
                .Carts.Include(c => c.CartItems)
                .ThenInclude(ci => ci.Product)
                .ThenInclude(p => p.Images)
                .FirstOrDefaultAsync(c => c.BuyerId == buyerId && c.UserId == null);
        }

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

        public async Task UpdateAsync(Cart cart)
        {
            cart.UpdatedAt = DateTime.UtcNow;
            _context.Carts.Update(cart);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Cart cart)
        {
            _context.Carts.Remove(cart);
            await _context.SaveChangesAsync();
        }

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

        public async Task RemoveItemAsync(CartItem cartItem)
        {
            _context.CartItems.Remove(cartItem);
            await _context.SaveChangesAsync();
        }

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

        // ✅ Todos los carritos (para depuración o mantenimiento)
        public async Task<List<Cart>> GetAllAsync()
        {
            return await _context
                .Carts.Include(c => c.User)
                .Include(c => c.CartItems)
                .ThenInclude(i => i.Product)
                .ThenInclude(p => p.Images)
                .ToListAsync();
        }

        // ✅ Carritos abandonados (configurable en appsettings.json y filtra usuarios semilla)
        public async Task<List<Cart>> GetAbandonedCartsAsync()
        {
            // Leer días de abandono desde appsettings.json (default: 3)
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
                    && c.User.IsSeed == false // ✅ Filtrar usuarios semilla
                )
                .ToListAsync();
        }
    }
}
