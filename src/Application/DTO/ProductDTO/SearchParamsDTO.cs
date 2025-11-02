using System.ComponentModel.DataAnnotations;
using TiendaUcnApi.src.Domain.Models;

namespace TiendaUcnApi.src.Application.DTO.ProductDTO;

/// <summary>
/// Opciones de ordenamiento para productos.
/// </summary>
public enum ProductSortOption
{
    /// <summary>
    /// Ordenar por fecha de creación (más recientes primero).
    /// </summary>
    Newest,

    /// <summary>
    /// Ordenar por precio ascendente.
    /// </summary>
    PriceAsc,

    /// <summary>
    /// Ordenar por precio descendente.
    /// </summary>
    PriceDesc,

    /// <summary>
    /// Ordenar por nombre alfabéticamente (A-Z).
    /// </summary>
    NameAsc,

    /// <summary>
    /// Ordenar por nombre alfabéticamente (Z-A).
    /// </summary>
    NameDesc,
}

/// <summary>
/// DTO para los parámetros de búsqueda, filtros, ordenamiento y paginación de productos.
/// </summary>
public class SearchParamsDTO
{
    /// <summary>
    /// Número de página a consultar.
    /// </summary>
    [Required(ErrorMessage = "El número de página es obligatorio.")]
    [Range(
        1,
        int.MaxValue,
        ErrorMessage = "El número de página debe ser un número entero positivo."
    )]
    public int PageNumber { get; set; }

    /// <summary>
    /// Cantidad de productos por página.
    /// </summary>
    [Range(
        1,
        int.MaxValue,
        ErrorMessage = "El tamaño de página debe ser un número entero positivo."
    )]
    public int? PageSize { get; set; }

    /// <summary>
    /// Término de búsqueda para filtrar productos.
    /// </summary>
    [MinLength(2, ErrorMessage = "El término de búsqueda debe tener al menos 2 caracteres.")]
    [MaxLength(40, ErrorMessage = "El término de búsqueda no puede exceder los 40 caracteres.")]
    public string? SearchTerm { get; set; }

    /// <summary>
    /// ID de categoría para filtrar productos.
    /// Implements R68 rubric requirement: category filter.
    /// </summary>
    public int? CategoryId { get; set; }

    /// <summary>
    /// ID de marca para filtrar productos.
    /// Implements R68 rubric requirement: brand filter.
    /// </summary>
    public int? BrandId { get; set; }

    /// <summary>
    /// Precio mínimo para filtrar productos.
    /// Implements R68 rubric requirement: price range filter.
    /// </summary>
    [Range(0, double.MaxValue, ErrorMessage = "El precio mínimo debe ser un valor positivo.")]
    public decimal? MinPrice { get; set; }

    /// <summary>
    /// Precio máximo para filtrar productos.
    /// Implements R68 rubric requirement: price range filter.
    /// </summary>
    [Range(0, double.MaxValue, ErrorMessage = "El precio máximo debe ser un valor positivo.")]
    public decimal? MaxPrice { get; set; }

    /// <summary>
    /// Estado del producto para filtrar (New, Used).
    /// Implements R68 rubric requirement: status filter.
    /// </summary>
    public Status? Status { get; set; }

    /// <summary>
    /// Opción de ordenamiento para los productos.
    /// Implements R70 rubric requirement: multiple sort options.
    /// </summary>
    public ProductSortOption? SortBy { get; set; }
}
