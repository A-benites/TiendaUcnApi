namespace TiendaUcnApi.src.Application.DTO.OrderDTO;

/// <summary>
/// DTO para representar un item de pedido.
/// </summary>
public class OrderItemDTO
{
    /// <summary>
    /// Identificador único del item.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Cantidad del producto.
    /// </summary>
    public int Quantity { get; set; }

    /// <summary>
    /// Precio del producto al momento del pedido.
    /// </summary>
    public decimal PriceAtMoment { get; set; }

    /// <summary>
    /// Título del producto al momento del pedido.
    /// </summary>
    public required string TitleAtMoment { get; set; }

    /// <summary>
    /// Descripción del producto al momento del pedido.
    /// </summary>
    public required string DescriptionAtMoment { get; set; }

    /// <summary>
    /// URL de la imagen del producto al momento del pedido.
    /// </summary>
    public required string ImageAtMoment { get; set; }

    /// <summary>
    /// Descuento aplicado al producto al momento del pedido.
    /// </summary>
    public decimal DiscountAtMoment { get; set; }
}
