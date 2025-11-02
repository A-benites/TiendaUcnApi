namespace TiendaUcnApi.src.Application.DTO.ProductDTO.ConsumerDTO;

/// <summary>
/// DTO para la respuesta de listado de productos con metadatos de paginación.
/// Implements R67 rubric requirement: complete pagination metadata.
/// </summary>
public class ProductListResponseDTO
{
    /// <summary>
    /// Lista de productos.
    /// </summary>
    public List<ListedProductsForCustomerDTO> Products { get; set; } =
        new List<ListedProductsForCustomerDTO>();

    /// <summary>
    /// Cantidad total de productos que coinciden con los filtros.
    /// </summary>
    public int TotalCount { get; set; }

    /// <summary>
    /// Número de página actual.
    /// </summary>
    public int Page { get; set; }

    /// <summary>
    /// Tamaño de página (cantidad de productos por página).
    /// </summary>
    public int PageSize { get; set; }

    /// <summary>
    /// Total de páginas disponibles.
    /// </summary>
    public int TotalPages { get; set; }
}
