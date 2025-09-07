using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TiendaUcnApi.src.Domain.Models;

public class OrderItem
{
    /// <summary>
    /// Unique identifier for the order item.
    /// </summary>
    [Key]
    public int Id { get; set; }

    /// <summary>
    /// Quantity of the item in the order.
    /// </summary>
    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "Product quantity must be at least 1.")]
    public int Quantity { get; set; }

    /// <summary>
    /// Price of the item at the time of the order.
    /// </summary>
    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal PriceAtMoment { get; set; }

    /// <summary>
    /// Title of the item at the time of the order.
    /// </summary>
    [Required]
    public string TitleAtMoment { get; set; }

    /// <summary>
    /// Description of the item at the time of the order.
    /// </summary>
    [Required]
    public string DescriptionAtMoment { get; set; }

    /// <summary>
    /// Image of the item at the time of the order (URL).
    /// </summary>
    [Required]
    public string ImageAtMoment { get; set; }

    /// <summary>
    /// Discount applied to the item at the time of the order.
    /// </summary>
    [Column(TypeName = "decimal(18,2)")]
    public decimal DiscountAtMoment { get; set; }

    // --- Relationship with Order ---
    /// <summary>
    /// Identifier of the order to which the item belongs.
    /// </summary>
    public int OrderId { get; set; }

    /// <summary>
    /// Order to which the item belongs (navigation property).
    /// </summary>
    [ForeignKey("OrderId")]
    public Order Order { get; set; } = null!;
}