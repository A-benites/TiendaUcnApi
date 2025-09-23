using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TiendaUcnApi.src.Domain.Models;

public class Order
{
    /// <summary>
    /// Unique identifier for the order.
    /// </summary>
    [Key]
    public int Id { get; set; }

    /// <summary>
    /// Unique order code.
    /// </summary>
    [Required]
    public required string Code { get; set; }

    /// <summary>
    /// Order total with discounts.
    /// </summary>
    [Required]
    [Range(0.01, (double)decimal.MaxValue, ErrorMessage = "Total must be a positive value.")]
    public decimal Total { get; set; }

    /// <summary>
    /// Order subtotal without discounts (sum of original prices).
    /// </summary>
    [Required]
    [Range(0.01, (double)decimal.MaxValue, ErrorMessage = "Subtotal must be a positive value.")]
    public decimal SubTotal { get; set; }

    /// <summary>
    /// Order creation date.
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Order update date.
    /// </summary>
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // --- Relationship with User ---
    /// <summary>
    /// Identifier of the user who placed the order.
    /// </summary>
    public int UserId { get; set; }

    /// <summary>
    /// User who placed the order (navigation property).
    /// </summary>
    [ForeignKey("UserId")]
    public User User { get; set; } = null!;

    /// <summary>
    /// List of items in the order.
    /// </summary>
    public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
}