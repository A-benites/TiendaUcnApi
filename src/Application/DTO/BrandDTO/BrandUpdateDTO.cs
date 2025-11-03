using System.ComponentModel.DataAnnotations;

namespace TiendaUcnApi.src.Application.DTO.BrandDTO;

/// <summary>
/// Data Transfer Object for updating an existing brand.
/// </summary>
public class BrandUpdateDTO
{
    /// <summary>
    /// New brand name. Must not exceed 50 characters.
    /// </summary>
    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [StringLength(50, ErrorMessage = "El nombre no puede superar los 50 caracteres.")]
    public required string Name { get; set; }
}
