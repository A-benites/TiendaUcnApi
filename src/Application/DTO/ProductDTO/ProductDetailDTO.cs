namespace TiendaUcnApi.src.Application.DTO.ProductDTO;

/// <summary>
/// DTO para mostrar el detalle de un producto.
/// </summary>
public class ProductDetailDTO
{
    /// <summary>
    /// Identificador único del producto.
    /// </summary>
    public required int Id { get; set; }

    /// <summary>
    /// Título del producto.
    /// </summary>
    public required string Title { get; set; }

    /// <summary>
    /// Descripción del producto.
    /// </summary>
    public required string Description { get; set; }

    /// <summary>
    /// Lista de URLs de imágenes del producto.
    /// </summary>
    public List<string> ImagesURL { get; set; } = new List<string>();

    /// <summary>
    /// Precio del producto.
    /// </summary>
    public required string Price { get; set; }

    /// <summary>
    /// Porcentaje de descuento aplicado.
    /// </summary>
    public required int Discount { get; set; }

    /// <summary>
    /// Precio final con descuento aplicado (calculado en servidor).
    /// Implements R76-R77 rubric requirement: finalPrice calculated server-side.
    /// </summary>
    public required string FinalPrice { get; set; }

    /// <summary>
    /// Stock disponible.
    /// </summary>
    public required int Stock { get; set; }

    /// <summary>
    /// Indicador de stock (por ejemplo, "Bajo", "Alto").
    /// </summary>
    public required string StockIndicator { get; set; }

    /// <summary>
    /// Nombre de la categoría del producto.
    /// </summary>
    public required string CategoryName { get; set; }

    /// <summary>
    /// Nombre de la marca del producto.
    /// </summary>
    public required string BrandName { get; set; }

    /// <summary>
    /// Estado del producto en texto.
    /// </summary>
    public required string StatusName { get; set; }

    /// <summary>
    /// Indica si el producto está disponible para la venta.
    /// </summary>
    public required bool IsAvailable { get; set; }
}
