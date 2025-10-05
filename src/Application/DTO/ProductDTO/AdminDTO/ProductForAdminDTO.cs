namespace TiendaUcnApi.src.Application.DTO.ProductDTO.AdminDTO;

/// <summary>
/// DTO para mostrar información de producto en administración.
/// </summary>
public class ProductForAdminDTO
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
    /// URL de la imagen principal del producto.
    /// </summary>
    public string? MainImageURL { get; set; }

    /// <summary>
    /// Precio del producto.
    /// </summary>
    public required string Price { get; set; }

    /// <summary>
    /// Stock disponible del producto.
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

    /// <summary>
    /// Fecha de última actualización del producto.
    /// </summary>
    public required DateTime UpdatedAt { get; set; }
}