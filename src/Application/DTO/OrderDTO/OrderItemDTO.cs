namespace TiendaUcnApi.src.Application.DTO.OrderDTO;

/// <summary>
/// Data Transfer Object representing an item in an order.
/// </summary>
public class OrderItemDTO
{
    /// <summary>
    /// Unique identifier for the order item.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Quantity of the product ordered.
    /// </summary>
    public int Quantity { get; set; }

    /// <summary>
    /// Price of the product at the time the order was placed.
    /// </summary>
    public decimal PriceAtMoment { get; set; }

    /// <summary>
    /// Title of the product at the time the order was placed.
    /// </summary>
    public required string TitleAtMoment { get; set; }

    /// <summary>
    /// Description of the product at the time the order was placed.
    /// </summary>
    public required string DescriptionAtMoment { get; set; }

    /// <summary>
    /// Image URL of the product at the time the order was placed.
    /// </summary>
    public required string ImageAtMoment { get; set; }

    /// <summary>
    /// Discount applied to the product at the time the order was placed.
    /// </summary>
    public decimal DiscountAtMoment { get; set; }
}
