using System.ComponentModel.DataAnnotations;

namespace TiendaUcnApi.src.Application.DTO;

/// <summary>
/// Data Transfer Object for user password change.
/// </summary>
public class ChangePasswordDTO
{
    /// <summary>
    /// User's current password.
    /// </summary>
    [Required(ErrorMessage = "La contraseña antigua es requerida.")]
    public required string OldPassword { get; set; } = string.Empty;

    /// <summary>
    /// New password the user wants to set.
    /// Must be at least 8 characters long and include uppercase, lowercase, numbers, and special characters.
    /// </summary>
    [Required(ErrorMessage = "La nueva contraseña es requerida.")]
    [MinLength(8, ErrorMessage = "La nueva contraseña debe tener al menos 8 caracteres.")]
    [RegularExpression(
        @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,}$",
        ErrorMessage = "La nueva contraseña debe contener al menos una mayúscula, una minúscula, un número y un carácter especial."
    )]
    public required string NewPassword { get; set; } = string.Empty;

    /// <summary>
    /// Confirmation of the new password. Must match the NewPassword field exactly.
    /// </summary>
    [Required(ErrorMessage = "La confirmación de contraseña es requerida.")]
    [Compare(
        nameof(NewPassword),
        ErrorMessage = "La nueva contraseña y su confirmación no coinciden."
    )]
    public required string ConfirmNewPassword { get; set; } = string.Empty;
}
