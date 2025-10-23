using System.ComponentModel.DataAnnotations;

namespace TiendaUcnApi.src.Application.DTO.BrandDTO;

public class BrandUpdateDTO
{
    /// <summary>
    /// Nuevo nombre de la marca.
    /// </summary>
    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [StringLength(50, ErrorMessage = "El nombre no puede superar los 50 caracteres.")]
    public required string Name { get; set; }
}
