using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TiendaUcnApi.src.Domain.Models;

public class Cart
{
    /// <summary>
    /// Identificador único del carrito de compras.
    /// </summary>
    [Key]
    public int Id { get; set; }

    /// <summary>
    /// Total del carrito de compras incluyendo descuentos.
    /// </summary>
    [Column(TypeName = "decimal(18,2)")]
    public decimal Total { get; set; }

    /// <summary>
    /// Subtotal del carrito de compras sin descuentos.
    /// </summary>
    [Column(TypeName = "decimal(18,2)")]
    public decimal SubTotal { get; set; }

    /// <summary>
    /// Identificador para el carrito de un usuario no autenticado.
    /// </summary>
    public string? BuyerId { get; set; }

    /// <summary>
    /// Fecha de creación del carrito de compras.
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Fecha de actualización del carrito de compras.
    /// </summary>
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // --- Relación con User ---
    /// <summary>
    /// Identificador del usuario que posee el carrito (si está autenticado).
    /// </summary>
    public int? UserId { get; set; }
    /// <summary>
    /// Usuario asociado al carrito (propiedad de navegación).
    /// </summary>
    [ForeignKey("UserId")]
    public User? User { get; set; }

    /// <summary>
    /// Lista de artículos en el carrito de compras.
    /// </summary>
    public ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
}