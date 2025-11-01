using System.ComponentModel.DataAnnotations;

namespace TiendaUcnApi.src.Application.DTO.BrandDTO;

public class BrandCreateDTO
{
    
    [Required(ErrorMessage = "El nombre de la marca es obligatorio.")]
    [StringLength(50, ErrorMessage = "El nombre no puede superar los 50 caracteres.")]
    public required string Name { get; set; }
}