using Mapster;
using Serilog;
using TiendaUcnApi.src.Application.DTO;
using TiendaUcnApi.src.Application.DTO.OrderDTO;
using TiendaUcnApi.src.Application.Services.Interfaces;
using TiendaUcnApi.src.Domain.Models;
using TiendaUcnApi.src.Infrastructure.Data;
using TiendaUcnApi.src.Infrastructure.Repositories.Interfaces;

namespace TiendaUcnApi.src.Application.Services.Implements;

/// <summary>
/// Service for managing customer orders.
/// Handles order creation with stock validation, transaction management, and order retrieval with filtering.
/// </summary>
public class OrderService : IOrderService
{
    /// <summary>
    /// Order repository for data access.
    /// </summary>
    private readonly IOrderRepository _orderRepository;

    /// <summary>
    /// Cart repository for accessing shopping cart data.
    /// </summary>
    private readonly ICartRepository _cartRepository;

    /// <summary>
    /// Product repository for stock management.
    /// </summary>
    private readonly IProductRepository _productRepository;

    /// <summary>
    /// Database context for transaction management.
    /// </summary>
    private readonly AppDbContext _dbContext;

    /// <summary>
    /// Initializes a new instance of the OrderService class.
    /// </summary>
    /// <param name="orderRepository">Order repository.</param>
    /// <param name="cartRepository">Cart repository.</param>
    /// <param name="productRepository">Product repository.</param>
    /// <param name="dbContext">Database context.</param>
    public OrderService(
        IOrderRepository orderRepository,
        ICartRepository cartRepository,
        IProductRepository productRepository,
        AppDbContext dbContext
    )
    {
        _orderRepository = orderRepository;
        _cartRepository = cartRepository;
        _productRepository = productRepository;
        _dbContext = dbContext;
    }

    /// <summary>
    /// Creates a new order from the user's cart.
    /// Implements R57: Uses database transactions to ensure atomicity (order creation, stock updates, cart deletion).
    /// Validates stock availability before creating the order.
    /// </summary>
    /// <param name="buyerId">Buyer identifier (can be anonymous or authenticated user ID).</param>
    /// <param name="userId">Authenticated user ID.</param>
    /// <returns>Generic response containing the created order DTO.</returns>
    /// <exception cref="InvalidOperationException">Thrown when cart is empty or stock is insufficient.</exception>
    public async Task<GenericResponse<OrderDTO>> CreateAsync(string buyerId, int userId)
    {
        // For authenticated users, prioritize finding by userId first, then by buyerId
        // This handles cases where authenticated users might not have the buyerId cookie
        var cart = await _cartRepository.GetByUserIdAsync(userId);

        // If no cart found by userId, try by buyerId (for users who shopped anonymously then logged in)
        if (cart == null)
        {
            cart = await _cartRepository.GetByBuyerIdAsync(buyerId);
        }

        if (cart == null || cart.CartItems.Count == 0)
        {
            Log.Information(
                "Attempt to create order with empty cart. BuyerId: {BuyerId}, UserId: {UserId}",
                buyerId,
                userId
            );
            throw new InvalidOperationException("The cart is empty");
        }

        // Validate stock for all products before creating the order
        foreach (var item in cart.CartItems)
        {
            var currentStock = await _productRepository.GetRealStockAsync(item.ProductId);
            if (currentStock < item.Quantity)
            {
                Log.Warning(
                    "Insufficient stock for product {ProductId}. Available stock: {Stock}, Requested quantity: {Quantity}",
                    item.ProductId,
                    currentStock,
                    item.Quantity
                );
                throw new InvalidOperationException(
                    $"Insufficient stock for product '{item.Product.Title}'. Available: {currentStock}"
                );
            }
        }

        // Calculate totals
        decimal subTotal = cart.CartItems.Sum(i => i.Product.Price * i.Quantity);
        decimal total = cart.CartItems.Sum(i =>
        {
            var itemTotal = i.Product.Price * i.Quantity;
            var discount = i.Product.Discount;
            return itemTotal * (1 - (decimal)discount / 100);
        });

        // Create the order
        var order = new Order
        {
            Code = $"ORD-{Guid.NewGuid().ToString("N").Substring(0, 10).ToUpper()}",
            UserId = userId,
            SubTotal = subTotal,
            Total = total,
            OrderItems = cart
                .CartItems.Select(i => new OrderItem
                {
                    Quantity = i.Quantity,
                    PriceAtMoment = i.Product.Price,
                    TitleAtMoment = i.Product.Title,
                    DescriptionAtMoment = i.Product.Description,
                    ImageAtMoment = i.Product.Images.FirstOrDefault()?.ImageUrl ?? "",
                    DiscountAtMoment = i.Product.Discount,
                })
                .ToList(),
        };

        // BEGIN TRANSACTION - Implements R57 rubric requirement
        // Ensures atomicity: all operations succeed or all fail
        using var transaction = await _dbContext.Database.BeginTransactionAsync();
        try
        {
            // 1. Create the order
            var createdOrder = await _orderRepository.CreateAsync(order);
            Log.Information(
                "Order created successfully. OrderId: {OrderId}, Code: {Code}",
                createdOrder.Id,
                createdOrder.Code
            );

            // 2. Update stock for all products
            foreach (var item in cart.CartItems)
            {
                var currentStock = await _productRepository.GetRealStockAsync(item.ProductId);
                await _productRepository.UpdateStockAsync(
                    item.ProductId,
                    currentStock - item.Quantity
                );
                Log.Information(
                    "Stock updated for product {ProductId}. Previous stock: {OldStock}, New stock: {NewStock}",
                    item.ProductId,
                    currentStock,
                    currentStock - item.Quantity
                );
            }

            // 3. Clear the cart
            await _cartRepository.DeleteAsync(cart);
            Log.Information("Cart deleted after creating order. CartId: {CartId}", cart.Id);

            // COMMIT TRANSACTION - All operations succeeded
            await transaction.CommitAsync();
            Log.Information(
                "Transaction completed successfully for order {OrderCode}",
                createdOrder.Code
            );

            return new GenericResponse<OrderDTO>(
                "Order created successfully",
                createdOrder.Adapt<OrderDTO>()
            );
        }
        catch (Exception ex)
        {
            // ROLLBACK TRANSACTION - An error occurred
            await transaction.RollbackAsync();
            Log.Error(
                ex,
                "Error creating order. Transaction rolled back. BuyerId: {BuyerId}, UserId: {UserId}",
                buyerId,
                userId
            );
            throw;
        }
    }

    /// <summary>
    /// Retrieves all orders for a specific user.
    /// </summary>
    /// <param name="userId">User ID.</param>
    /// <returns>Generic response containing list of order DTOs.</returns>
    public async Task<GenericResponse<List<OrderDTO>>> GetAllByUser(int userId)
    {
        var orders = await _orderRepository.GetAllByUser(userId);
        var ordersDto = orders.Adapt<List<OrderDTO>>();

        Log.Information(
            "Orders retrieved for user {UserId}. Total: {Count}",
            userId,
            ordersDto.Count
        );

        return new GenericResponse<List<OrderDTO>>("Orders retrieved successfully", ordersDto);
    }

    /// <summary>
    /// Retrieves paginated orders for a specific user with filtering.
    /// </summary>
    /// <param name="userId">User ID.</param>
    /// <param name="filter">Filter parameters including page and page size.</param>
    /// <returns>Generic response containing paginated order list DTO.</returns>
    public async Task<GenericResponse<OrderListDTO>> GetAllByUserPaginated(
        int userId,
        UserOrderFilterDTO filter
    )
    {
        var (orders, totalCount) = await _orderRepository.GetAllByUserPaginated(
            userId,
            filter.Page,
            filter.PageSize
        );

        var ordersDto = orders.Adapt<List<OrderDTO>>();
        var totalPages = (int)Math.Ceiling(totalCount / (double)filter.PageSize);

        var result = new OrderListDTO
        {
            Orders = ordersDto,
            TotalCount = totalCount,
            CurrentPage = filter.Page,
            PageSize = filter.PageSize,
            TotalPages = totalPages,
        };

        Log.Information(
            "Paginated orders retrieved for user {UserId}. Total: {TotalCount}, Page: {Page}, Size: {PageSize}",
            userId,
            totalCount,
            filter.Page,
            filter.PageSize
        );

        return new GenericResponse<OrderListDTO>("Orders retrieved successfully", result);
    }

    /// <summary>
    /// Retrieves detailed information for a specific order.
    /// Validates that the order belongs to the requesting user.
    /// </summary>
    /// <param name="orderId">Order ID.</param>
    /// <param name="userId">Requesting user ID.</param>
    /// <returns>Generic response containing order detail DTO.</returns>
    /// <exception cref="KeyNotFoundException">Thrown when order is not found.</exception>
    /// <exception cref="UnauthorizedAccessException">Thrown when user doesn't own the order.</exception>
    public async Task<GenericResponse<OrderDTO>> GetOrderDetailByIdAsync(int orderId, int userId)
    {
        var order = await _orderRepository.GetByIdAsync(orderId);

        if (order == null)
        {
            Log.Warning("Order not found. OrderId: {OrderId}", orderId);
            throw new KeyNotFoundException($"Order with ID {orderId} not found.");
        }

        // Verify that the order belongs to the user
        if (order.UserId != userId)
        {
            Log.Warning(
                "User {UserId} attempted to access order {OrderId} that does not belong to them.",
                userId,
                orderId
            );
            throw new UnauthorizedAccessException(
                "You do not have permission to access this order."
            );
        }

        var orderDto = order.Adapt<OrderDTO>();
        Log.Information(
            "Order detail obtained. OrderId: {OrderId}, UserId: {UserId}",
            orderId,
            userId
        );

        return new GenericResponse<OrderDTO>("Order detail obtained successfully", orderDto);
    }

    /// <summary>
    /// Retrieves all orders with filtering and pagination for administrators.
    /// </summary>
    /// <param name="filter">Filter parameters including status, user, dates, sorting, and pagination.</param>
    /// <returns>Generic response containing paginated order list DTO.</returns>
    public async Task<GenericResponse<OrderListDTO>> GetAllOrdersAsync(OrderFilterDTO filter)
    {
        var (orders, totalCount) = await _orderRepository.GetAllAsync(filter);

        var ordersDto = orders.Adapt<List<OrderDTO>>();
        var totalPages = (int)Math.Ceiling(totalCount / (double)filter.PageSize);

        var result = new OrderListDTO
        {
            Orders = ordersDto,
            TotalCount = totalCount,
            CurrentPage = filter.Page,
            PageSize = filter.PageSize,
            TotalPages = totalPages,
        };

        Log.Information(
            "Orders obtained for admin. Total: {TotalCount}, Page: {Page}",
            totalCount,
            filter.Page
        );

        return new GenericResponse<OrderListDTO>("Orders obtained successfully", result);
    }

    /// <summary>
    /// Retrieves detailed information for a specific order (admin access).
    /// </summary>
    /// <param name="orderId">Order ID.</param>
    /// <returns>Generic response containing order detail DTO.</returns>
    /// <exception cref="KeyNotFoundException">Thrown when order is not found.</exception>
    public async Task<GenericResponse<OrderDTO>> GetOrderByIdAsync(int orderId)
    {
        var order = await _orderRepository.GetByIdAsync(orderId);

        if (order == null)
        {
            Log.Warning("Order not found. OrderId: {OrderId}", orderId);
            throw new KeyNotFoundException($"Order with ID {orderId} not found.");
        }

        var orderDto = order.Adapt<OrderDTO>();
        Log.Information("Order detail obtained by admin. OrderId: {OrderId}", orderId);

        return new GenericResponse<OrderDTO>("Order detail obtained successfully", orderDto);
    }

    /// <summary>
    /// Updates the status of an order.
    /// Validates status transitions using OrderStatusTransitionValidator (R123).
    /// Creates an audit record of the status change.
    /// </summary>
    /// <param name="orderId">Order ID.</param>
    /// <param name="dto">DTO containing the new status.</param>
    /// <param name="adminId">ID of the administrator performing the update.</param>
    /// <returns>Generic response containing updated order DTO.</returns>
    public async Task<GenericResponse<OrderDTO>> UpdateOrderStatusAsync(
        int orderId,
        UpdateOrderStatusDTO dto,
        int adminId
    )
    {
        var order = await _orderRepository.UpdateStatusAsync(orderId, dto.Status, adminId);

        Log.Information(
            "Order status updated. OrderId: {OrderId}, AdminId: {AdminId}, New status: {Status}",
            orderId,
            adminId,
            dto.Status
        );

        var orderDto = order.Adapt<OrderDTO>();
        return new GenericResponse<OrderDTO>("Order status updated successfully", orderDto);
    }
}
