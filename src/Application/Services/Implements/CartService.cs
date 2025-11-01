using Mapster;
using Serilog;
using TiendaUcnApi.src.Domain.Models;
using TiendaUcnApi.src.Application.DTO.CartDTO;
using TiendaUcnApi.src.Application.Services.Interfaces;
using TiendaUcnApi.src.Infrastructure.Repositories.Interfaces;

namespace TiendaUcnApi.src.Application.Services.Implements
{
    /// <summary>
    /// Service implementation for managing shopping cart operations.
    /// </summary>
    public class CartService : ICartService
    {
        private readonly ICartRepository _cartRepository;
        private readonly IProductRepository _productRepository;

        public CartService(ICartRepository cartRepository, IProductRepository productRepository)
        {
            _cartRepository = cartRepository;
            _productRepository = productRepository;
        }

        public async Task<CartDTO> AddItemAsync(string buyerId, int productId, int quantity, int? userId = null)
        {
            Cart? cart = await _cartRepository.FindAsync(buyerId, userId);
            Product? product = await _productRepository.GetByIdAsync(productId);

            if (product == null)
                throw new KeyNotFoundException("El producto no existe.");

            if (product.Stock < quantity)
                throw new ArgumentException("No hay suficiente stock del producto.");

            if (cart == null)
                cart = await _cartRepository.CreateAsync(buyerId, userId);

            var existingProduct = cart.CartItems.FirstOrDefault(ci => ci.ProductId == productId);
            if (existingProduct != null)
                existingProduct.Quantity += quantity;
            else
            {
                var newCartItem = new CartItem
                {
                    ProductId = product.Id,
                    Product = product,
                    CartId = cart.Id,
                    Quantity = quantity,
                };
                await _cartRepository.AddItemAsync(cart, newCartItem);
            }

            RecalculateCartTotals(cart);
            await _cartRepository.UpdateAsync(cart);
            cart = await _cartRepository.FindAsync(cart.BuyerId!, cart.UserId);

            return cart.Adapt<CartDTO>();
        }

        public async Task<CartDTO> ClearAsync(string buyerId, int? userId = null)
        {
            Cart? cart = await _cartRepository.FindAsync(buyerId, userId);
            if (cart == null)
                throw new KeyNotFoundException("El carrito no existe para el comprador especificado.");

            cart.CartItems.Clear();
            RecalculateCartTotals(cart);
            await _cartRepository.UpdateAsync(cart);

            return cart.Adapt<CartDTO>();
        }

        public async Task<CartDTO> CreateOrGetAsync(string buyerId, int? userId = null)
        {
            Cart? cart = await _cartRepository.FindAsync(buyerId, userId);
            if (cart == null)
            {
                if (userId.HasValue)
                {
                    var existingUserCart = await _cartRepository.GetByUserIdAsync(userId.Value);
                    if (existingUserCart != null)
                        return existingUserCart.Adapt<CartDTO>();
                }

                cart = await _cartRepository.CreateAsync(buyerId, userId);
            }

            return cart.Adapt<CartDTO>();
        }

        public async Task<CartDTO> RemoveItemAsync(string buyerId, int productId, int? userId = null)
        {
            Cart? cart = await _cartRepository.FindAsync(buyerId, userId);
            if (cart == null)
                throw new KeyNotFoundException("El carrito no existe para el comprador especificado.");

            CartItem? itemToRemove = cart.CartItems.FirstOrDefault(ci => ci.ProductId == productId);
            if (itemToRemove == null)
                throw new KeyNotFoundException("El artículo no existe en el carrito.");

            cart.CartItems.Remove(itemToRemove);
            await _cartRepository.RemoveItemAsync(itemToRemove);
            RecalculateCartTotals(cart);
            await _cartRepository.UpdateAsync(cart);

            return cart.Adapt<CartDTO>();
        }

        public async Task AssociateWithUserAsync(string buyerId, int userId)
        {
            Cart? cart = await _cartRepository.GetAnonymousAsync(buyerId);
            if (cart == null) return;

            var existingUserCart = await _cartRepository.GetByUserIdAsync(userId);

            if (existingUserCart != null)
            {
                foreach (var anonymousItem in cart.CartItems)
                {
                    var existingItem = existingUserCart.CartItems.FirstOrDefault(i => i.ProductId == anonymousItem.ProductId);
                    if (existingItem != null)
                        existingItem.Quantity += anonymousItem.Quantity;
                    else
                    {
                        anonymousItem.CartId = existingUserCart.Id;
                        existingUserCart.CartItems.Add(anonymousItem);
                    }
                }

                RecalculateCartTotals(existingUserCart);
                await _cartRepository.UpdateAsync(existingUserCart);
                await _cartRepository.DeleteAsync(cart);
            }
            else
            {
                cart.UserId = userId;
                await _cartRepository.UpdateAsync(cart);
            }
        }

        public async Task<CartDTO> UpdateItemQuantityAsync(string buyerId, int productId, int quantity, int? userId = null)
        {
            Cart? cart = await _cartRepository.FindAsync(buyerId, userId);
            if (cart == null)
                throw new KeyNotFoundException("El carrito no existe para el comprador especificado.");

            var product = await _productRepository.GetByIdAsync(productId);
            if (product == null)
                throw new KeyNotFoundException("El producto no existe para el ID especificado.");

            var itemToUpdate = cart.CartItems.FirstOrDefault(ci => ci.ProductId == productId);
            if (itemToUpdate == null)
                throw new KeyNotFoundException("Producto del carrito no encontrado");

            if (product.Stock < quantity)
                throw new ArgumentException("Stock insuficiente");

            itemToUpdate.Quantity = quantity;
            RecalculateCartTotals(cart);
            await _cartRepository.UpdateAsync(cart);

            return cart.Adapt<CartDTO>();
        }

        private static void RecalculateCartTotals(Cart cart)
        {
            if (!cart.CartItems.Any())
            {
                cart.SubTotal = 0;
                cart.Total = 0;
                return;
            }

            cart.SubTotal = cart.CartItems.Sum(ci => ci.Product.Price * ci.Quantity);
            cart.Total = cart.CartItems.Sum(ci =>
            {
                var discount = ci.Product.Discount;
                var itemTotal = ci.Product.Price * ci.Quantity;
                return (int)(itemTotal * (1 - (decimal)discount / 100));
            });
        }

        public async Task<CartDTO> CheckoutAsync(string buyerId, int? userId)
        {
            Cart? cart = await _cartRepository.FindAsync(buyerId, userId);
            if (cart == null)
                throw new KeyNotFoundException("El carrito no existe para el comprador especificado.");

            if (!cart.CartItems.Any())
                throw new InvalidOperationException("El carrito está vacío.");

            var itemsToRemove = new List<CartItem>();
            var itemsToUpdate = new List<(CartItem item, int newQuantity)>();
            bool hasChanges = false;

            foreach (var item in cart.CartItems.ToList())
            {
                int productStock = await _productRepository.GetRealStockAsync(item.ProductId);

                if (productStock == 0)
                {
                    itemsToRemove.Add(item);
                    hasChanges = true;
                }
                else if (item.Quantity > productStock)
                {
                    itemsToUpdate.Add((item, productStock));
                    hasChanges = true;
                }
            }

            if (hasChanges)
            {
                foreach (var item in itemsToRemove)
                {
                    cart.CartItems.Remove(item);
                    await _cartRepository.RemoveItemAsync(item);
                }

                foreach (var (item, newQuantity) in itemsToUpdate)
                    item.Quantity = newQuantity;

                RecalculateCartTotals(cart);
                await _cartRepository.UpdateAsync(cart);
            }

            return cart.Adapt<CartDTO>();
        }

        /// <summary>
        /// Returns carts that have not been updated in the last X days.
        /// Used for Hangfire background tasks to send abandoned cart reminders.
        /// </summary>
        public async Task<IEnumerable<CartDTO>> GetAbandonedCartsAsync(int daysInactive)
        {
            var cutoffDate = DateTime.UtcNow.AddDays(-daysInactive);
            var carts = await _cartRepository.GetAllAsync();

            var abandonedCarts = carts
                .Where(c => c.UpdatedAt < cutoffDate && c.CartItems.Any())
                .Select(c => c.Adapt<CartDTO>())
                .ToList();

            return abandonedCarts;
        }
    }
}
