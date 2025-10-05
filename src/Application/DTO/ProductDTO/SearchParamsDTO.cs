using System.ComponentModel.DataAnnotations;

namespace TiendaUcnApi.src.Application.DTO.ProductDTO;

/// <summary>
/// DTO para los parámetros de búsqueda y paginación de productos.
/// </summary>
public class SearchParamsDTO
{
    /// <summary>
    /// Número de página a consultar.
    /// </summary>
    [Required(ErrorMessage = "El número de página es obligatorio.")]
    [Range(1, int.MaxValue, ErrorMessage = "El número de página debe ser un número entero positivo.")]
    public int PageNumber { get; set; }

    /// <summary>
    /// Cantidad de productos por página.
    /// </summary>
    [Range(1, int.MaxValue, ErrorMessage = "El tamaño de página debe ser un número entero positivo.")]
    public int? PageSize { get; set; }

    /// <summary>
    /// Término de búsqueda para filtrar productos.
    /// </summary>
    [MinLength(2, ErrorMessage = "El término de búsqueda debe tener al menos 2 caracteres.")]
    [MaxLength(40, ErrorMessage = "El término de búsqueda no puede exceder los 40 caracteres.")]
    public string? SearchTerm { get; set; }
}