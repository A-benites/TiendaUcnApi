using System.ComponentModel.DataAnnotations;

namespace TiendaUcnApi.src.Application.DTO.BrandDTO;

/// <summary>
/// Data Transfer Object for creating a new brand.
/// </summary>
public class BrandCreateDTO
{
    /// <summary>
    /// Brand name. Must not exceed 50 characters.
    /// </summary>
    [Required(ErrorMessage = "El nombre de la marca es obligatorio.")]
    [StringLength(50, ErrorMessage = "El nombre no puede superar los 50 caracteres.")]
    public required string Name { get; set; }
}
