using System.ComponentModel.DataAnnotations;

namespace TiendaUcnApi.src.Application.DTO;
public class LoginDTO
{
    [Required(ErrorMessage = "El email es requerido.")]
    [EmailAddress(ErrorMessage = "El Correo electrónico no es válido.")]
    public required string Email { get; set; }

    [Required(ErrorMessage = "La contraseña es requerida.")]
    public required string Password { get; set; }

    public bool RememberMe { get; set; }
}