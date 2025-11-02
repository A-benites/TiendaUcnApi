using System.ComponentModel.DataAnnotations;

namespace TiendaUcnApi.src.Application.DTO;

/// <summary>
/// DTO para el cambio de contraseña de usuario.
/// </summary>
public class ChangePasswordDTO
{
    /// <summary>
    /// Contraseña actual del usuario.
    /// </summary>
    [Required(ErrorMessage = "La contraseña antigua es requerida.")]
    public required string OldPassword { get; set; } = string.Empty;

    /// <summary>
    /// Nueva contraseña que el usuario desea establecer.
    /// Debe tener al menos 8 caracteres, incluir mayúsculas, minúsculas, números y caracteres especiales.
    /// </summary>
    [Required(ErrorMessage = "La nueva contraseña es requerida.")]
    [MinLength(8, ErrorMessage = "La nueva contraseña debe tener al menos 8 caracteres.")]
    [RegularExpression(
        @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,}$",
        ErrorMessage = "La nueva contraseña debe contener al menos una mayúscula, una minúscula, un número y un carácter especial."
    )]
    public required string NewPassword { get; set; } = string.Empty;

    /// <summary>
    /// Confirmación de la nueva contraseña.
    /// Debe coincidir exactamente con NewPassword.
    /// </summary>
    [Required(ErrorMessage = "La confirmación de contraseña es requerida.")]
    [Compare(
        nameof(NewPassword),
        ErrorMessage = "La nueva contraseña y su confirmación no coinciden."
    )]
    public required string ConfirmNewPassword { get; set; } = string.Empty;
}
