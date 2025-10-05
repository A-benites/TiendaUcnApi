namespace TiendaUcnApi.Src.Application.DTO.ProductDTO.CustomerDTO;

/// <summary>
/// DTO para mostrar información de producto al cliente.
/// </summary>
public class ProductForCustomerDTO
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
    /// URL de la imagen principal del producto.
    /// </summary>
    public required string MainImageURL { get; set; }

    /// <summary>
    /// Precio del producto.
    /// </summary>
    public required string Price { get; set; }

    /// <summary>
    /// Porcentaje de descuento aplicado.
    /// </summary>
    public required int Discount { get; set; }
}