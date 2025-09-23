using System.ComponentModel.DataAnnotations;

namespace TiendaUcnApi.src.Application.DTO;
public class ProfileDTO
{
    [Required(ErrorMessage = "El email es requerido.")]
    [EmailAddress(ErrorMessage = "El Correo electrónico no es válido.")]
    public required string Email { get; set; }

    [Required(ErrorMessage = "El primer nombre es requerido")]
    public required string FirstName { get; set; }
}