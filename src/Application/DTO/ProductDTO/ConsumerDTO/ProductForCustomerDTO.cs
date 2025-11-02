namespace TiendaUcnApi.Src.Application.DTO.ProductDTO.CustomerDTO;

/// <summary>
/// DTO for displaying product information to customers.
/// </summary>
public class ProductForCustomerDTO
{
    /// <summary>
    /// Unique product identifier.
    /// </summary>
    public required int Id { get; set; }

    /// <summary>
    /// Product title.
    /// </summary>
    public required string Title { get; set; }

    /// <summary>
    /// Product description.
    /// </summary>
    public required string Description { get; set; }

    /// <summary>
    /// Main product image URL.
    /// </summary>
    public required string MainImageURL { get; set; }

    /// <summary>
    /// Product price.
    /// </summary>
    public required string Price { get; set; }

    /// <summary>
    /// Discount percentage applied.
    /// </summary>
    public required int Discount { get; set; }
}
