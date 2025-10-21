using Mapster;
using TiendaUcnApi.src.Application.DTO.OrderDTO;
using TiendaUcnApi.src.Domain.Models;

namespace TiendaUcnApi.src.Application.Mappers;

/// <summary>
/// Configuración de mapeos para órdenes usando Mapster.
/// </summary>
public class OrderMapper
{
    /// <summary>
    /// Configura todos los mapeos relacionados con órdenes.
    /// </summary>
    public void ConfigureAllMappings()
    {
        ConfigureOrderMappings();
        ConfigureOrderItemMappings();
    }

    /// <summary>
    /// Configura el mapeo de Order a OrderDTO.
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
            .Map(dest => dest.OrderItems, src => src.OrderItems);
    }

    /// <summary>
    /// Configura el mapeo de OrderItem a OrderItemDTO.
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
