namespace TiendaUcnApi.src.Application.DTO.BrandDTO;

public class BrandDTO
{
    /// <summary>
    /// Identificador único de la marca.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Nombre de la marca.
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// Fecha de creación de la marca.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Cantidad de productos asociados a esta marca.
    /// </summary>
    public int ProductCount { get; set; }
}
