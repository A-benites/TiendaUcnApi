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

        /// <summary>
        /// Initializes a new instance of the CartService class.
        /// </summary>
        /// <param name="cartRepository">Cart repository.</param>
        /// <param name="productRepository">Product repository.</param>
        public CartService(ICartRepository cartRepository, IProductRepository productRepository)
        {
            _cartRepository = cartRepository;
            _productRepository = productRepository;
        }

        /// <summary>
        /// Adds an item to the shopping cart or updates quantity if it already exists.
        /// </summary>
        /// <param name="buyerId">Buyer identifier (anonymous or user ID).</param>
        /// <param name="productId">Product ID to add.</param>
        /// <param name="quantity">Quantity to add.</param>
        /// <param name="userId">Optional authenticated user ID.</param>
        /// <returns>Updated cart data.</returns>
        /// <exception cref="KeyNotFoundException">Thrown when product doesn't exist.</exception>
        /// <exception cref="ArgumentException">Thrown when insufficient stock.</exception>
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
                throw new KeyNotFoundException("The product does not exist.");

            if (product.Stock < quantity)
                throw new ArgumentException("There is not enough stock for the product.");

            if (cart == null)
                cart = await _cartRepository.CreateAsync(buyerId, userId);

            var existingProduct = cart.CartItems.FirstOrDefault(ci => ci.ProductId == productId);
            if (existingProduct != null)
            {
                Log.Information(
                    "CartService: Updating existing product quantity. BuyerId: {BuyerId}, UserId: {UserId}, ProductId: {ProductId}, PreviousQuantity: {PreviousQuantity}, NewQuantity: {NewQuantity}",
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
                    "CartService: Item added to cart. BuyerId: {BuyerId}, UserId: {UserId}, ProductId: {ProductId}, Quantity: {Quantity}, ProductTitle: {ProductTitle}",
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

        /// <summary>
        /// Clears all items from the shopping cart.
        /// </summary>
        /// <param name="buyerId">Buyer identifier.</param>
        /// <param name="userId">Optional authenticated user ID.</param>
        /// <returns>Empty cart data.</returns>
        /// <exception cref="KeyNotFoundException">Thrown when cart doesn't exist.</exception>
        public async Task<CartDTO> ClearAsync(string buyerId, int? userId = null)
        {
            Cart? cart = await _cartRepository.FindAsync(buyerId, userId);
            if (cart == null)
                throw new KeyNotFoundException("The cart does not exist for the specified buyer.");

            var itemCount = cart.CartItems.Count;
            cart.CartItems.Clear();
            RecalculateCartTotals(cart);
            await _cartRepository.UpdateAsync(cart);

            Log.Information(
                "CartService: Cart cleared. BuyerId: {BuyerId}, UserId: {UserId}, ItemsDeleted: {ItemsDeleted}",
                buyerId,
                userId,
                itemCount
            );

            return cart.Adapt<CartDTO>();
        }

        /// <summary>
        /// Creates a new cart or retrieves an existing one.
        /// Automatically validates and removes unavailable products from existing carts.
        /// </summary>
        /// <param name="buyerId">Buyer identifier.</param>
        /// <param name="userId">Optional authenticated user ID.</param>
        /// <returns>Cart data with list of removed unavailable products (if any).</returns>
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
        /// <param name="cart">Cart to validate.</param>
        /// <param name="buyerId">Buyer identifier.</param>
        /// <param name="userId">Optional user ID.</param>
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
                        "CartService: Item removed from cart due to inactive or deleted product. BuyerId: {BuyerId}, UserId: {UserId}, ProductId: {ProductId}",
                        buyerId,
                        userId,
                        item.ProductId
                    );
                }

                RecalculateCartTotals(cart);
                await _cartRepository.UpdateAsync(cart);

                Log.Information(
                    "CartService: {Count} unavailable products removed from cart. BuyerId: {BuyerId}, UserId: {UserId}",
                    itemsToRemove.Count,
                    buyerId,
                    userId
                );
            }

            return removedProductIds;
        }

        /// <summary>
        /// Removes a specific item from the shopping cart.
        /// </summary>
        /// <param name="buyerId">Buyer identifier.</param>
        /// <param name="productId">Product ID to remove.</param>
        /// <param name="userId">Optional authenticated user ID.</param>
        /// <returns>Updated cart data.</returns>
        /// <exception cref="KeyNotFoundException">Thrown when cart or item doesn't exist.</exception>
        public async Task<CartDTO> RemoveItemAsync(
            string buyerId,
            int productId,
            int? userId = null
        )
        {
            Cart? cart = await _cartRepository.FindAsync(buyerId, userId);
            if (cart == null)
                throw new KeyNotFoundException("The cart does not exist for the specified buyer.");

            CartItem? itemToRemove = cart.CartItems.FirstOrDefault(ci => ci.ProductId == productId);
            if (itemToRemove == null)
                throw new KeyNotFoundException("The item does not exist in the cart.");

            cart.CartItems.Remove(itemToRemove);
            await _cartRepository.RemoveItemAsync(itemToRemove);
            RecalculateCartTotals(cart);
            await _cartRepository.UpdateAsync(cart);

            Log.Information(
                "CartService: Item removed from cart. BuyerId: {BuyerId}, UserId: {UserId}, ProductId: {ProductId}, QuantityDeleted: {QuantityDeleted}",
                buyerId,
                userId,
                productId,
                itemToRemove.Quantity
            );

            return cart.Adapt<CartDTO>();
        }

        /// <summary>
        /// Associates an anonymous cart with an authenticated user.
        /// Merges anonymous cart items with existing user cart if one exists.
        /// </summary>
        /// <param name="buyerId">Anonymous buyer identifier.</param>
        /// <param name="userId">Authenticated user ID.</param>
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
                    "CartService: Anonymous cart merged with user cart. BuyerId: {BuyerId}, UserId: {UserId}, ItemsMerged: {ItemsMerged}, ItemsAdded: {ItemsAdded}",
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
                    "CartService: Anonymous cart associated with user. BuyerId: {BuyerId}, UserId: {UserId}, ItemsCount: {ItemsCount}",
                    buyerId,
                    userId,
                    cart.CartItems.Count
                );
            }
        }

        /// <summary>
        /// Updates the quantity of a specific item in the cart.
        /// </summary>
        /// <param name="buyerId">Buyer identifier.</param>
        /// <param name="productId">Product ID to update.</param>
        /// <param name="quantity">New quantity value.</param>
        /// <param name="userId">Optional authenticated user ID.</param>
        /// <returns>Updated cart data.</returns>
        /// <exception cref="KeyNotFoundException">Thrown when cart, product, or item doesn't exist.</exception>
        /// <exception cref="ArgumentException">Thrown when insufficient stock.</exception>
        public async Task<CartDTO> UpdateItemQuantityAsync(
            string buyerId,
            int productId,
            int quantity,
            int? userId = null
        )
        {
            Cart? cart = await _cartRepository.FindAsync(buyerId, userId);
            if (cart == null)
                throw new KeyNotFoundException("The cart does not exist for the specified buyer.");

            var product = await _productRepository.GetByIdAsync(productId);
            if (product == null)
                throw new KeyNotFoundException("The product does not exist for the specified ID.");

            var itemToUpdate = cart.CartItems.FirstOrDefault(ci => ci.ProductId == productId);
            if (itemToUpdate == null)
                throw new KeyNotFoundException("Cart product not found");

            if (product.Stock < quantity)
                throw new ArgumentException("Insufficient stock");

            var oldQuantity = itemToUpdate.Quantity;
            itemToUpdate.Quantity = quantity;
            RecalculateCartTotals(cart);
            await _cartRepository.UpdateAsync(cart);

            Log.Information(
                "CartService: Item quantity updated. BuyerId: {BuyerId}, UserId: {UserId}, ProductId: {ProductId}, PreviousQuantity: {PreviousQuantity}, NewQuantity: {NewQuantity}",
                buyerId,
                userId,
                productId,
                oldQuantity,
                quantity
            );

            return cart.Adapt<CartDTO>();
        }

        /// <summary>
        /// Recalculates cart subtotal and total with discounts.
        /// </summary>
        /// <param name="cart">Cart to recalculate.</param>
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

        /// <summary>
        /// Validates and prepares the cart for checkout.
        /// Removes out-of-stock and unavailable products, adjusts quantities based on available stock.
        /// Implements R48 rubric requirement: validate product availability and stock at checkout.
        /// </summary>
        /// <param name="buyerId">Buyer identifier.</param>
        /// <param name="userId">Optional authenticated user ID.</param>
        /// <returns>Validated cart data ready for order creation.</returns>
        /// <exception cref="KeyNotFoundException">Thrown when cart doesn't exist.</exception>
        /// <exception cref="InvalidOperationException">Thrown when cart is empty.</exception>
        public async Task<CartDTO> CheckoutAsync(string buyerId, int? userId)
        {
            Cart? cart = await _cartRepository.FindAsync(buyerId, userId);
            if (cart == null)
                throw new KeyNotFoundException("The cart does not exist for the specified buyer.");

            if (!cart.CartItems.Any())
                throw new InvalidOperationException("The cart is empty.");

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
                        "CartService: Item removed during checkout due to inactive or unavailable product. BuyerId: {BuyerId}, UserId: {UserId}, ProductId: {ProductId}",
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
                        "CartService: Item removed during checkout due to lack of stock. BuyerId: {BuyerId}, UserId: {UserId}, ProductId: {ProductId}",
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
                        "CartService: Item quantity adjusted during checkout due to limited stock. BuyerId: {BuyerId}, UserId: {UserId}, ProductId: {ProductId}, PreviousQuantity: {PreviousQuantity}, NewQuantity: {NewQuantity}",
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
