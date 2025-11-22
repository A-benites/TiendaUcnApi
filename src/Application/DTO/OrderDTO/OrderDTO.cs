namespace TiendaUcnApi.src.Application.DTO.OrderDTO;

/// <summary>
/// Data Transfer Object representing an order.
/// </summary>
public class OrderDTO
{
    /// <summary>
    /// Unique identifier for the order.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Unique order code.
    /// </summary>
    public required string Code { get; set; }

    /// <summary>
    /// Total amount of the order with discounts applied.
    /// </summary>
    public decimal Total { get; set; }

    /// <summary>
    /// Subtotal amount of the order without discounts.
    /// </summary>
    public decimal SubTotal { get; set; }

    /// <summary>
    /// Date and time when the order was created.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Current status of the order (Pending, Paid, Shipped, Delivered, Cancelled).
    /// </summary>
    public string Status { get; set; } = "Pending";

    /// <summary>
    /// ID of the user who placed the order.
    /// </summary>
    public int UserId { get; set; }

    /// <summary>
    /// Email of the user who placed the order.
    /// </summary>
    public string? UserEmail { get; set; }

    /// <summary>
    /// Full name of the user who placed the order.
    /// </summary>
    public string? UserName { get; set; }

    /// <summary>
    /// List of items in the order.
    /// </summary>
    public List<OrderItemDTO> OrderItems { get; set; } = new List<OrderItemDTO>();
}
