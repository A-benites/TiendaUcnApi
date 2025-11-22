using Mapster;
using TiendaUcnApi.src.Application.DTO.OrderDTO;
using TiendaUcnApi.src.Domain.Models;

namespace TiendaUcnApi.src.Application.Mappers;

/// <summary>
/// Configures mappings between Order entities and Order DTOs using Mapster.
/// Handles transformations for order display with historical product information.
/// </summary>
public class OrderMapper
{
    /// <summary>
    /// Configures all order-related mappings.
    /// </summary>
    public void ConfigureAllMappings()
    {
        ConfigureOrderMappings();
        ConfigureOrderItemMappings();
    }

    /// <summary>
    /// Configures the mapping from Order entity to OrderDTO.
    /// Maps order identification, pricing totals, timestamps, status, user info, and associated order items.
    /// </summary>
    private void ConfigureOrderMappings()
    {
        TypeAdapterConfig<Order, OrderDTO>
            .NewConfig()
            .Map(dest => dest.Id, src => src.Id)
            .Map(dest => dest.Code, src => src.Code)
            .Map(dest => dest.Total, src => src.Total)
            .Map(dest => dest.SubTotal, src => src.SubTotal)
            .Map(dest => dest.CreatedAt, src => src.CreatedAt)
            .Map(dest => dest.Status, src => src.Status.ToString())
            .Map(dest => dest.UserId, src => src.UserId)
            .Map(dest => dest.UserEmail, src => src.User != null ? src.User.Email : null)
            .Map(
                dest => dest.UserName,
                src => src.User != null ? $"{src.User.FirstName} {src.User.LastName}" : null
            )
            .Map(dest => dest.OrderItems, src => src.OrderItems);
    }

    /// <summary>
    /// Configures the mapping from OrderItem entity to OrderItemDTO.
    /// Maps historical product data captured at the moment of order creation,
    /// including price, title, description, image, and discount applied.
    /// </summary>
    private void ConfigureOrderItemMappings()
    {
        TypeAdapterConfig<OrderItem, OrderItemDTO>
            .NewConfig()
            .Map(dest => dest.Id, src => src.Id)
            .Map(dest => dest.Quantity, src => src.Quantity)
            .Map(dest => dest.PriceAtMoment, src => src.PriceAtMoment)
            .Map(dest => dest.TitleAtMoment, src => src.TitleAtMoment)
            .Map(dest => dest.DescriptionAtMoment, src => src.DescriptionAtMoment)
            .Map(dest => dest.ImageAtMoment, src => src.ImageAtMoment)
            .Map(dest => dest.DiscountAtMoment, src => src.DiscountAtMoment);
    }
}
