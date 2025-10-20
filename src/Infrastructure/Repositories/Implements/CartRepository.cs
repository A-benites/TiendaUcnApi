using Microsoft.EntityFrameworkCore;
using System.Globalization;
using TiendaUcnApi.src.Domain.Models;
using TiendaUcnApi.src.Infrastructure.Data;
using TiendaUcnApi.src.Infrastructure.Repositories.Interfaces;


namespace TiendaUcnApi.src.Infrastructure.Repositories.Implements
{

    public class CartRepository : ICartRepository
    {

        private readonly AppDbContext _context;

        public CartRepository(AppDbContext dataContext)
        {
            _context = dataContext;
        }


        public async Task<Cart?> FindAsync(string buyerId, int? userId)
        {
            Cart? cart = null;

            if (userId.HasValue)
            {
                cart = await _context.Carts.Include(c => c.CartItems).ThenInclude(ci => ci.Product).ThenInclude(p => p.Images).FirstOrDefaultAsync(c => c.UserId == userId);


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
            cart = await _context.Carts.Include(c => c.CartItems)
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
            return await _context.Carts.Include(c => c.CartItems)
                                            .ThenInclude(ci => ci.Product)
                                        .FirstOrDefaultAsync(c => c.UserId == userId);
        }

        public async Task<Cart?> GetAnonymousAsync(string buyerId)
        {
            return await _context.Carts.Include(c => c.CartItems)
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
                UpdatedAt = DateTime.UtcNow
            };

            _context.Carts.Add(cart);
            await _context.SaveChangesAsync();

            return await _context.Carts.Include(c => c.CartItems)
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
            // Aseguramos que el producto ya existe en la BD
            _context.Attach(cartItem.Product);

            // También adjuntamos sus relaciones si existen
            if (cartItem.Product.Brand != null)
                _context.Attach(cartItem.Product.Brand);
            if (cartItem.Product.Category != null)
                _context.Attach(cartItem.Product.Category);

            // Adjuntamos el carrito también, por si no está trackeado
            _context.Attach(cart);

            // Finalmente agregamos el ítem
            _context.CartItems.Add(cartItem);

            await _context.SaveChangesAsync();
        }

        public async Task RemoveItemAsync(CartItem cartItem)
        {
            _context.CartItems.Remove(cartItem);
            await _context.SaveChangesAsync();
        }



    }


}