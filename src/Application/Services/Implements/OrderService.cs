using Mapster;
using Serilog;
using TiendaUcnApi.src.Application.DTO;
using TiendaUcnApi.src.Application.DTO.OrderDTO;
using TiendaUcnApi.src.Application.Services.Interfaces;
using TiendaUcnApi.src.Domain.Models;
using TiendaUcnApi.src.Infrastructure.Repositories.Interfaces;

namespace TiendaUcnApi.src.Application.Services.Implements;

public class OrderService : IOrderService
{
    private readonly IOrderRepository _orderRepository;
    private readonly ICartRepository _cartRepository;
    private readonly IProductRepository _productRepository;

    public OrderService(
        IOrderRepository orderRepository,
        ICartRepository cartRepository,
        IProductRepository productRepository
    )
    {
        _orderRepository = orderRepository;
        _cartRepository = cartRepository;
        _productRepository = productRepository;
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

        var createdOrder = await _orderRepository.CreateAsync(order);
        Log.Information(
            "Orden creada exitosamente. OrderId: {OrderId}, Code: {Code}",
            createdOrder.Id,
            createdOrder.Code
        );

        // Actualizar stock de productos
        foreach (var item in cart.CartItems)
        {
            var currentStock = await _productRepository.GetRealStockAsync(item.ProductId);
            await _productRepository.UpdateStockAsync(item.ProductId, currentStock - item.Quantity);
        }

        // Limpiar el carrito
        await _cartRepository.DeleteAsync(cart);
        Log.Information("Carrito eliminado después de crear la orden. CartId: {CartId}", cart.Id);

        return new GenericResponse<OrderDTO>(
            "Orden creada exitosamente",
            createdOrder.Adapt<OrderDTO>()
        );
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
}
