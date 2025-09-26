namespace TiendaUcnApi.src.Application.DTO.ProductDTO.AdminDTO;

/// <summary>
/// DTO que representa una lista paginada de productos para administración.
/// </summary>
public class ListedProductsForAdminDTO
{
    /// <summary>
    /// Lista de productos para administración.
    /// </summary>
    public List<ProductForAdminDTO> Products { get; set; } = new List<ProductForAdminDTO>();

    /// <summary>
    /// Cantidad total de productos encontrados.
    /// </summary>
    public int TotalCount { get; set; }

    /// <summary>
    /// Número total de páginas disponibles.
    /// </summary>
    public int TotalPages { get; set; }

    /// <summary>
    /// Página actual de la consulta.
    /// </summary>
    public int CurrentPage { get; set; }

    /// <summary>
    /// Cantidad de productos por página.
    /// </summary>
    public int PageSize { get; set; }
}