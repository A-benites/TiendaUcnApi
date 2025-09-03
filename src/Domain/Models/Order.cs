using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TiendaUcnApi.src.Domain.Models;

public class Order
{
    /// <summary>
    /// Identificador único del pedido.
    /// </summary>
    [Key]
    public int Id { get; set; }

    /// <summary>
    /// Código único del pedido.
    /// </summary>
    // La columna en la base de datos no puede ser nula.
    [Required]
    public string Code { get; set; }

    /// <summary>
    /// Total del pedido con descuentos.
    /// </summary>
    // La columna en la base de datos no puede ser nula.
    [Required]
    // Define el tipo de dato en la BD para mayor precisión con valores monetarios.
    [Column(TypeName = "decimal(18,2)")]
    public decimal Total { get; set; }

    /// <summary>
    /// Subtotal del pedido sin descuentos (suma de precios originales).
    /// </summary>
    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal SubTotal { get; set; }

    /// <summary>
    /// Fecha de creación del pedido.
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Fecha de actualización del pedido.
    /// </summary>
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // --- Relación con User ---
    /// <summary>
    /// Identificador del usuario que realizó el pedido.
    /// </summary>
    public int UserId { get; set; }

    /// <summary>
    /// Usuario que realizó el pedido (propiedad de navegación).
    /// </summary>
    [ForeignKey("UserId")]
    public User User { get; set; } = null!;

    /// <summary>
    /// Lista de artículos del pedido.
    /// </summary>
    public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
}