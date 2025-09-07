using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TiendaUcnApi.src.Domain.Models;

public class CartItem
{
    /// <summary>
    /// Unique identifier for the cart item.
    /// </summary>
    [Key]
    public int Id { get; set; }

    /// <summary>
    /// Quantity of the product in the cart.
    /// </summary>
    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "Product quantity must be at least 1.")]
    public int Quantity { get; set; }

    // --- Relationship with Product ---
    /// <summary>
    /// Id of the product associated with the cart item.
    /// </summary>
    public int ProductId { get; set; }
    /// <summary>
    /// Product associated with the item (navigation property).
    /// </summary>
    [ForeignKey("ProductId")]
    public Product Product { get; set; } = null!;

    // --- Relationship with Cart ---
    /// <summary>
    /// Id of the cart to which the item belongs.
    /// </summary>
    public int CartId { get; set; }
    /// <summary>
    /// Cart to which the item belongs (navigation property).
    /// </summary>
    [ForeignKey("CartId")]
    public Cart Cart { get; set; } = null!;
}