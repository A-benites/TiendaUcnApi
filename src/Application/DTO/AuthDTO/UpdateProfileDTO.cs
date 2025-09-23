using System.ComponentModel.DataAnnotations;

namespace TiendaUcnApi.src.Application.DTO;

public class UpdateProfileDTO
{
    [Required(ErrorMessage = "El primer nombre es requerido.")]
    public required string FirstName { get; set; }
}