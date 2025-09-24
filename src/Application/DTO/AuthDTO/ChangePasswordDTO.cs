using System.ComponentModel.DataAnnotations;

namespace TiendaUcnApi.src.Application.DTO;

public class ChangePasswordDTO
{
    [Required(ErrorMessage = "La contraseña actual es requerida.")]
    public required string OldPassword { get; set; }

    [Required(ErrorMessage = "La nueva contraseña es requerida.")]
    [StringLength(
        20,
        MinimumLength = 8,
        ErrorMessage = "La contraseña debe tener entre 8 y 20 caracteres."
    )]
    [RegularExpression(
        @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$",
        ErrorMessage = "La contraseña debe tener al menos una mayúscula, una minúscula, un número y un carácter especial."
    )]
    public required string NewPassword { get; set; }

    [Required(ErrorMessage = "La confirmación de la contraseña es obligatoria.")]
    [Compare("NewPassword", ErrorMessage = "Las contraseñas no coinciden.")]
    public required string ConfirmPassword { get; set; }
}