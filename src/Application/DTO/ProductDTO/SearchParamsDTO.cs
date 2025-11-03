using System.ComponentModel.DataAnnotations;
using TiendaUcnApi.src.Domain.Models;

namespace TiendaUcnApi.src.Application.DTO.ProductDTO;

/// <summary>
/// Sorting options for products.
/// </summary>
public enum ProductSortOption
{
    /// <summary>
    /// Sort by creation date (newest first).
    /// </summary>
    Newest,

    /// <summary>
    /// Sort by price in ascending order.
    /// </summary>
    PriceAsc,

    /// <summary>
    /// Sort by price in descending order.
    /// </summary>
    PriceDesc,

    /// <summary>
    /// Sort by name alphabetically (A-Z).
    /// </summary>
    NameAsc,

    /// <summary>
    /// Sort by name alphabetically (Z-A).
    /// </summary>
    NameDesc,
}

/// <summary>
/// Data Transfer Object for product search parameters, filters, sorting, and pagination.
/// </summary>
public class SearchParamsDTO
{
    /// <summary>
    /// Page number to retrieve.
    /// </summary>
    [Required(ErrorMessage = "El número de página es obligatorio.")]
    [Range(
        1,
        int.MaxValue,
        ErrorMessage = "El número de página debe ser un número entero positivo."
    )]
    public int PageNumber { get; set; }

    /// <summary>
    /// Number of products per page.
    /// </summary>
    [Range(
        1,
        int.MaxValue,
        ErrorMessage = "El tamaño de página debe ser un número entero positivo."
    )]
    public int? PageSize { get; set; }

    /// <summary>
    /// Search term to filter products by title or description.
    /// </summary>
    [MinLength(2, ErrorMessage = "El término de búsqueda debe tener al menos 2 caracteres.")]
    [MaxLength(40, ErrorMessage = "El término de búsqueda no puede exceder los 40 caracteres.")]
    public string? SearchTerm { get; set; }

    /// <summary>
    /// Category ID to filter products.
    /// Implements R68 rubric requirement: category filter.
    /// </summary>
    public int? CategoryId { get; set; }

    /// <summary>
    /// Brand ID to filter products.
    /// Implements R68 rubric requirement: brand filter.
    /// </summary>
    public int? BrandId { get; set; }

    /// <summary>
    /// Minimum price to filter products.
    /// Implements R68 rubric requirement: price range filter.
    /// </summary>
    [Range(0, double.MaxValue, ErrorMessage = "El precio mínimo debe ser un valor positivo.")]
    public decimal? MinPrice { get; set; }

    /// <summary>
    /// Maximum price to filter products.
    /// Implements R68 rubric requirement: price range filter.
    /// </summary>
    [Range(0, double.MaxValue, ErrorMessage = "El precio máximo debe ser un valor positivo.")]
    public decimal? MaxPrice { get; set; }

    /// <summary>
    /// Product status to filter (New, Used).
    /// Implements R68 rubric requirement: status filter.
    /// </summary>
    public Status? Status { get; set; }

    /// <summary>
    /// Sorting option for products.
    /// Implements R70 rubric requirement: multiple sort options.
    /// </summary>
    public ProductSortOption? SortBy { get; set; }
}
