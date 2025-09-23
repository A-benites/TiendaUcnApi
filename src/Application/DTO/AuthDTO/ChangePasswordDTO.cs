using System.ComponentModel.DataAnnotations;

namespace TiendaUcnApi.src.Application.DTO;
public class ChangePasswordDTO
{
    [Required(ErrorMessage = "La contraseña antigua es requerida.")]
    public required string OldPassword { get; set; } = string.Empty;

    [Required(ErrorMessage = "La nueva contraseña es requerida.")]
    public required string NewPassword { get; set; } = string.Empty;
}