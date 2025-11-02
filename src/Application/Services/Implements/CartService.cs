using Mapster;
using Serilog;
using TiendaUcnApi.src.Application.DTO.CartDTO;
using TiendaUcnApi.src.Application.Services.Interfaces;
using TiendaUcnApi.src.Domain.Models;
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

        public async Task<CartDTO> AddItemAsync(
            string buyerId,
            int productId,
            int quantity,
            int? userId = null
        )
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
            {
                Log.Information(
                    "CartService: Actualizando cantidad de producto existente. BuyerId: {BuyerId}, UserId: {UserId}, ProductId: {ProductId}, CantidadAnterior: {CantidadAnterior}, CantidadNueva: {CantidadNueva}",
                    buyerId,
                    userId,
                    productId,
                    existingProduct.Quantity,
                    existingProduct.Quantity + quantity
                );
                existingProduct.Quantity += quantity;
            }
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
                Log.Information(
                    "CartService: Item agregado al carrito. BuyerId: {BuyerId}, UserId: {UserId}, ProductId: {ProductId}, Cantidad: {Cantidad}, ProductTitle: {ProductTitle}",
                    buyerId,
                    userId,
                    productId,
                    quantity,
                    product.Title
                );
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
                throw new KeyNotFoundException(
                    "El carrito no existe para el comprador especificado."
                );

            var itemCount = cart.CartItems.Count;
            cart.CartItems.Clear();
            RecalculateCartTotals(cart);
            await _cartRepository.UpdateAsync(cart);

            Log.Information(
                "CartService: Carrito vaciado. BuyerId: {BuyerId}, UserId: {UserId}, ItemsEliminados: {ItemsEliminados}",
                buyerId,
                userId,
                itemCount
            );

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
                    {
                        var removedProducts = await ValidateAndCleanUnavailableProductsAsync(
                            existingUserCart,
                            buyerId,
                            userId
                        );
                        var cartDto = existingUserCart.Adapt<CartDTO>();
                        cartDto.RemovedUnavailableProducts = removedProducts;
                        return cartDto;
                    }
                }

                cart = await _cartRepository.CreateAsync(buyerId, userId);
            }
            else
            {
                var removedProducts = await ValidateAndCleanUnavailableProductsAsync(
                    cart,
                    buyerId,
                    userId
                );
                var cartDto = cart.Adapt<CartDTO>();
                cartDto.RemovedUnavailableProducts = removedProducts;
                return cartDto;
            }

            return cart.Adapt<CartDTO>();
        }

        /// <summary>
        /// Validates and removes unavailable products from the cart.
        /// Implements R49 rubric requirement: exclude unavailable products when viewing cart.
        /// </summary>
        /// <returns>List of product IDs that were removed.</returns>
        private async Task<List<int>> ValidateAndCleanUnavailableProductsAsync(
            Cart cart,
            string buyerId,
            int? userId
        )
        {
            var itemsToRemove = new List<CartItem>();

            foreach (var item in cart.CartItems.ToList())
            {
                var product = await _productRepository.GetByIdAsync(item.ProductId);

                // Remove items if product doesn't exist or is not available
                if (product == null || !product.IsAvailable)
                {
                    itemsToRemove.Add(item);
                }
            }

            var removedProductIds = new List<int>();

            if (itemsToRemove.Any())
            {
                foreach (var item in itemsToRemove)
                {
                    removedProductIds.Add(item.ProductId);
                    cart.CartItems.Remove(item);
                    await _cartRepository.RemoveItemAsync(item);

                    Log.Information(
                        "CartService: Item eliminado del carrito por producto inactivo o eliminado. BuyerId: {BuyerId}, UserId: {UserId}, ProductId: {ProductId}",
                        buyerId,
                        userId,
                        item.ProductId
                    );
                }

                RecalculateCartTotals(cart);
                await _cartRepository.UpdateAsync(cart);

                Log.Information(
                    "CartService: Se eliminaron {Count} productos no disponibles del carrito. BuyerId: {BuyerId}, UserId: {UserId}",
                    itemsToRemove.Count,
                    buyerId,
                    userId
                );
            }

            return removedProductIds;
        }

        public async Task<CartDTO> RemoveItemAsync(
            string buyerId,
            int productId,
            int? userId = null
        )
        {
            Cart? cart = await _cartRepository.FindAsync(buyerId, userId);
            if (cart == null)
                throw new KeyNotFoundException(
                    "El carrito no existe para el comprador especificado."
                );

            CartItem? itemToRemove = cart.CartItems.FirstOrDefault(ci => ci.ProductId == productId);
            if (itemToRemove == null)
                throw new KeyNotFoundException("El artículo no existe en el carrito.");

            cart.CartItems.Remove(itemToRemove);
            await _cartRepository.RemoveItemAsync(itemToRemove);
            RecalculateCartTotals(cart);
            await _cartRepository.UpdateAsync(cart);

            Log.Information(
                "CartService: Item eliminado del carrito. BuyerId: {BuyerId}, UserId: {UserId}, ProductId: {ProductId}, CantidadEliminada: {CantidadEliminada}",
                buyerId,
                userId,
                productId,
                itemToRemove.Quantity
            );

            return cart.Adapt<CartDTO>();
        }

        public async Task AssociateWithUserAsync(string buyerId, int userId)
        {
            Cart? cart = await _cartRepository.GetAnonymousAsync(buyerId);
            if (cart == null)
                return;

            var existingUserCart = await _cartRepository.GetByUserIdAsync(userId);

            if (existingUserCart != null)
            {
                var itemsMerged = 0;
                var itemsAdded = 0;

                foreach (var anonymousItem in cart.CartItems)
                {
                    var existingItem = existingUserCart.CartItems.FirstOrDefault(i =>
                        i.ProductId == anonymousItem.ProductId
                    );
                    if (existingItem != null)
                    {
                        existingItem.Quantity += anonymousItem.Quantity;
                        itemsMerged++;
                    }
                    else
                    {
                        anonymousItem.CartId = existingUserCart.Id;
                        existingUserCart.CartItems.Add(anonymousItem);
                        itemsAdded++;
                    }
                }

                RecalculateCartTotals(existingUserCart);
                await _cartRepository.UpdateAsync(existingUserCart);
                await _cartRepository.DeleteAsync(cart);

                Log.Information(
                    "CartService: Carrito anónimo fusionado con carrito de usuario. BuyerId: {BuyerId}, UserId: {UserId}, ItemsFusionados: {ItemsMerged}, ItemsAgregados: {ItemsAdded}",
                    buyerId,
                    userId,
                    itemsMerged,
                    itemsAdded
                );
            }
            else
            {
                cart.UserId = userId;
                await _cartRepository.UpdateAsync(cart);

                Log.Information(
                    "CartService: Carrito anónimo asociado a usuario. BuyerId: {BuyerId}, UserId: {UserId}, ItemsCount: {ItemsCount}",
                    buyerId,
                    userId,
                    cart.CartItems.Count
                );
            }
        }

        public async Task<CartDTO> UpdateItemQuantityAsync(
            string buyerId,
            int productId,
            int quantity,
            int? userId = null
        )
        {
            Cart? cart = await _cartRepository.FindAsync(buyerId, userId);
            if (cart == null)
                throw new KeyNotFoundException(
                    "El carrito no existe para el comprador especificado."
                );

            var product = await _productRepository.GetByIdAsync(productId);
            if (product == null)
                throw new KeyNotFoundException("El producto no existe para el ID especificado.");

            var itemToUpdate = cart.CartItems.FirstOrDefault(ci => ci.ProductId == productId);
            if (itemToUpdate == null)
                throw new KeyNotFoundException("Producto del carrito no encontrado");

            if (product.Stock < quantity)
                throw new ArgumentException("Stock insuficiente");

            var oldQuantity = itemToUpdate.Quantity;
            itemToUpdate.Quantity = quantity;
            RecalculateCartTotals(cart);
            await _cartRepository.UpdateAsync(cart);

            Log.Information(
                "CartService: Cantidad de item actualizada. BuyerId: {BuyerId}, UserId: {UserId}, ProductId: {ProductId}, CantidadAnterior: {CantidadAnterior}, CantidadNueva: {CantidadNueva}",
                buyerId,
                userId,
                productId,
                oldQuantity,
                quantity
            );

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
                throw new KeyNotFoundException(
                    "El carrito no existe para el comprador especificado."
                );

            if (!cart.CartItems.Any())
                throw new InvalidOperationException("El carrito está vacío.");

            var itemsToRemove = new List<CartItem>();
            var itemsToRemoveUnavailable = new List<CartItem>();
            var itemsToUpdate = new List<(CartItem item, int newQuantity)>();
            bool hasChanges = false;

            foreach (var item in cart.CartItems.ToList())
            {
                var product = await _productRepository.GetByIdAsync(item.ProductId);

                // Remove if product doesn't exist or is not available
                if (product == null || !product.IsAvailable)
                {
                    itemsToRemoveUnavailable.Add(item);
                    hasChanges = true;
                    continue;
                }

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
                // Remove unavailable products first
                foreach (var item in itemsToRemoveUnavailable)
                {
                    cart.CartItems.Remove(item);
                    await _cartRepository.RemoveItemAsync(item);
                    Log.Information(
                        "CartService: Item eliminado durante checkout por producto inactivo o no disponible. BuyerId: {BuyerId}, UserId: {UserId}, ProductId: {ProductId}",
                        buyerId,
                        userId,
                        item.ProductId
                    );
                }

                // Remove out of stock products
                foreach (var item in itemsToRemove)
                {
                    cart.CartItems.Remove(item);
                    await _cartRepository.RemoveItemAsync(item);
                    Log.Information(
                        "CartService: Item eliminado durante checkout por falta de stock. BuyerId: {BuyerId}, UserId: {UserId}, ProductId: {ProductId}",
                        buyerId,
                        userId,
                        item.ProductId
                    );
                }

                foreach (var (item, newQuantity) in itemsToUpdate)
                {
                    var oldQuantity = item.Quantity;
                    item.Quantity = newQuantity;
                    Log.Information(
                        "CartService: Cantidad de item ajustada durante checkout por stock limitado. BuyerId: {BuyerId}, UserId: {UserId}, ProductId: {ProductId}, CantidadAnterior: {CantidadAnterior}, CantidadNueva: {CantidadNueva}",
                        buyerId,
                        userId,
                        item.ProductId,
                        oldQuantity,
                        newQuantity
                    );
                }

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
