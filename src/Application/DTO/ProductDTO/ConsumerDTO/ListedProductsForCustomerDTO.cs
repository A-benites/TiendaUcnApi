namespace TiendaUcnApi.src.Application.DTO.ProductDTO.ConsumerDTO;

/// <summary>
/// DTO para mostrar información resumida de productos al cliente en listas.
/// </summary>
public class ListedProductsForCustomerDTO
{
    /// <summary>
    /// Identificador único del producto.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Título del producto.
    /// </summary>
    public required string Title { get; set; }

    /// <summary>
    /// Descripción del producto.
    /// </summary>
    public required string Description { get; set; }

    /// <summary>
    /// URL de la imagen principal del producto.
    /// </summary>
    public string? MainImageURL { get; set; }

    /// <summary>
    /// Precio original del producto.
    /// </summary>
    public decimal Price { get; set; }

    /// <summary>
    /// Porcentaje de descuento aplicado.
    /// </summary>
    public int Discount { get; set; }

    /// <summary>
    /// Precio final con descuento aplicado (calculado en servidor).
    /// Implements R71-R72 rubric requirement: finalPrice calculated server-side.
    /// </summary>
    public decimal FinalPrice { get; set; }

    /// <summary>
    /// Stock disponible del producto.
    /// </summary>
    public int Stock { get; set; }

    /// <summary>
    /// Nombre de la categoría del producto.
    /// </summary>
    public string? CategoryName { get; set; }

    /// <summary>
    /// Nombre de la marca del producto.
    /// </summary>
    public string? BrandName { get; set; }
}
