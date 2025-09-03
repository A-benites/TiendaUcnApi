using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TiendaUcnApi.src.Domain.Models;

public class CartItem
{
    /// <summary>
    /// Identificador único del artículo en el carrito de compras.
    /// </summary>
    [Key]
    public int Id { get; set; }

    /// <summary>
    /// Cantidad del producto en el carrito de compras.
    /// </summary>
    [Required]
    public int Quantity { get; set; }

    // --- Relación con Product ---
    /// <summary>
    /// Id del producto asociado al artículo del carrito.
    /// </summary>
    public int ProductId { get; set; }
    /// <summary>
    /// Producto asociado al artículo (propiedad de navegación).
    /// </summary>
    [ForeignKey("ProductId")]
    public Product Product { get; set; } = null!;

    // --- Relación con Cart ---
    /// <summary>
    /// Id del carrito de compras al que pertenece el artículo.
    /// </summary>
    public int CartId { get; set; }
    /// <summary>
    /// Carrito de compras al que pertenece el artículo (propiedad de navegación).
    /// </summary>
    [ForeignKey("CartId")]
    public Cart Cart { get; set; } = null!;
}