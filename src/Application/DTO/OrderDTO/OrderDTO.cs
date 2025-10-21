namespace TiendaUcnApi.src.Application.DTO.OrderDTO;

/// <summary>
/// DTO para representar un pedido.
/// </summary>
public class OrderDTO
{
    /// <summary>
    /// Identificador único del pedido.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Código único del pedido.
    /// </summary>
    public required string Code { get; set; }

    /// <summary>
    /// Total del pedido con descuentos.
    /// </summary>
    public decimal Total { get; set; }

    /// <summary>
    /// Subtotal del pedido sin descuentos.
    /// </summary>
    public decimal SubTotal { get; set; }

    /// <summary>
    /// Fecha de creación del pedido.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Lista de items en el pedido.
    /// </summary>
    public List<OrderItemDTO> OrderItems { get; set; } = new List<OrderItemDTO>();
}
