using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TiendaUcnApi.src.Domain.Models;

public class OrderItem
{
    /// <summary>
    /// Identificador único del artículo del pedido.
    /// </summary>
    [Key]
    public int Id { get; set; }

    /// <summary>
    /// Cantidad del artículo en el pedido.
    /// </summary>
    [Required]
    public int Quantity { get; set; }

    /// <summary>
    /// Precio del artículo en el momento del pedido.
    /// </summary>
    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal PriceAtMoment { get; set; }

    /// <summary>
    /// Título del artículo en el momento del pedido.
    /// </summary>
    [Required]
    public string TitleAtMoment { get; set; }

    /// <summary>
    /// Descripción del artículo en el momento del pedido.
    /// </summary>
    [Required]
    public string DescriptionAtMoment { get; set; }

    /// <summary>
    /// Imagen del artículo en el momento del pedido (URL).
    /// </summary>
    [Required]
    public string ImageAtMoment { get; set; }

    /// <summary>
    /// Descuento aplicado al artículo al momento del pedido.
    /// </summary>
    [Column(TypeName = "decimal(18,2)")]
    public decimal DiscountAtMoment { get; set; }

    // --- Relación con Order ---
    /// <summary>
    /// Identificador del pedido al que pertenece el artículo.
    /// </summary>
    public int OrderId { get; set; }

    /// <summary>
    /// Pedido al que pertenece el artículo (propiedad de navegación).
    /// </summary>
    [ForeignKey("OrderId")]
    public Order Order { get; set; } = null!;
}