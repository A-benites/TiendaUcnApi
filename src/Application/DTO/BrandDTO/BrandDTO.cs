namespace TiendaUcnApi.src.Application.DTO.BrandDTO;

/// <summary>
/// Data Transfer Object representing a brand in the API.
/// </summary>
public class BrandDTO
{
    /// <summary>
    /// Unique identifier for the brand.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Brand name.
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// Date when the brand was created.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Total number of products associated with this brand.
    /// </summary>
    public int ProductCount { get; set; }
}
