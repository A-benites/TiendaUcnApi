using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TiendaUcnApi.src.Domain.Models;

public class Cart
{
    /// <summary>
    /// Unique identifier for the shopping cart.
    /// </summary>
    [Key]
    public int Id { get; set; }

    /// <summary>
    /// Cart total including discounts.
    /// </summary>
    [Column(TypeName = "decimal(18,2)")]
    public decimal Total { get; set; }

    /// <summary>
    /// Cart subtotal without discounts.
    /// </summary>
    [Column(TypeName = "decimal(18,2)")]
    public decimal SubTotal { get; set; }

    /// <summary>
    /// Identifier for the cart of an unauthenticated user.
    /// </summary>
    public string? BuyerId { get; set; }

    /// <summary>
    /// Cart creation date.
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Cart update date.
    /// </summary>
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public int? UserId { get; set; }
    [ForeignKey("UserId")]
    public User? User { get; set; }

    /// <summary>
    /// List of items in the shopping cart.
    /// </summary>
    public ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
}