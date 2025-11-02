using Mapster;
using Serilog;
using TiendaUcnApi.src.Application.DTO;
using TiendaUcnApi.src.Application.DTO.OrderDTO;
using TiendaUcnApi.src.Application.Services.Interfaces;
using TiendaUcnApi.src.Domain.Models;
using TiendaUcnApi.src.Infrastructure.Data;
using TiendaUcnApi.src.Infrastructure.Repositories.Interfaces;

namespace TiendaUcnApi.src.Application.Services.Implements;

public class OrderService : IOrderService
{
    private readonly IOrderRepository _orderRepository;
    private readonly ICartRepository _cartRepository;
    private readonly IProductRepository _productRepository;
    private readonly AppDbContext _dbContext;

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

    public async Task<GenericResponse<OrderDTO>> CreateAsync(string buyerId, int userId)
    {
        var cart = await _cartRepository.GetByBuyerIdAsync(buyerId);

        if (cart == null || cart.CartItems.Count == 0)
        {
            Log.Information(
                "Intento de crear orden con carrito vacío. BuyerId: {BuyerId}",
                buyerId
            );
            throw new InvalidOperationException("El carrito está vacío");
        }

        // Validar stock de todos los productos antes de crear la orden
        foreach (var item in cart.CartItems)
        {
            var currentStock = await _productRepository.GetRealStockAsync(item.ProductId);
            if (currentStock < item.Quantity)
            {
                Log.Warning(
                    "Stock insuficiente para el producto {ProductId}. Stock disponible: {Stock}, Cantidad solicitada: {Quantity}",
                    item.ProductId,
                    currentStock,
                    item.Quantity
                );
                throw new InvalidOperationException(
                    $"Stock insuficiente para el producto '{item.Product.Title}'. Disponible: {currentStock}"
                );
            }
        }

        // Calcular totales
        decimal subTotal = cart.CartItems.Sum(i => i.Product.Price * i.Quantity);
        decimal total = cart.CartItems.Sum(i =>
        {
            var itemTotal = i.Product.Price * i.Quantity;
            var discount = i.Product.Discount;
            return itemTotal * (1 - (decimal)discount / 100);
        });

        // Crear la orden
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
                "Orden creada exitosamente. OrderId: {OrderId}, Code: {Code}",
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
                    "Stock actualizado para producto {ProductId}. Stock anterior: {OldStock}, Nuevo stock: {NewStock}",
                    item.ProductId,
                    currentStock,
                    currentStock - item.Quantity
                );
            }

            // 3. Clear the cart
            await _cartRepository.DeleteAsync(cart);
            Log.Information(
                "Carrito eliminado después de crear la orden. CartId: {CartId}",
                cart.Id
            );

            // COMMIT TRANSACTION - All operations succeeded
            await transaction.CommitAsync();
            Log.Information(
                "Transacción completada exitosamente para orden {OrderCode}",
                createdOrder.Code
            );

            return new GenericResponse<OrderDTO>(
                "Orden creada exitosamente",
                createdOrder.Adapt<OrderDTO>()
            );
        }
        catch (Exception ex)
        {
            // ROLLBACK TRANSACTION - An error occurred
            await transaction.RollbackAsync();
            Log.Error(
                ex,
                "Error al crear orden. Transacción revertida. BuyerId: {BuyerId}, UserId: {UserId}",
                buyerId,
                userId
            );
            throw;
        }
    }

    public async Task<GenericResponse<List<OrderDTO>>> GetAllByUser(int userId)
    {
        var orders = await _orderRepository.GetAllByUser(userId);
        var ordersDto = orders.Adapt<List<OrderDTO>>();

        Log.Information(
            "Órdenes obtenidas para el usuario {UserId}. Total: {Count}",
            userId,
            ordersDto.Count
        );

        return new GenericResponse<List<OrderDTO>>("Órdenes obtenidas exitosamente", ordersDto);
    }

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
            "Órdenes paginadas obtenidas para el usuario {UserId}. Total: {TotalCount}, Página: {Page}, Tamaño: {PageSize}",
            userId,
            totalCount,
            filter.Page,
            filter.PageSize
        );

        return new GenericResponse<OrderListDTO>("Órdenes obtenidas exitosamente", result);
    }

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
