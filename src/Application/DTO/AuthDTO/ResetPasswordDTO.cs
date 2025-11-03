using System.ComponentModel.DataAnnotations;

namespace TiendaUcnApi.src.Application.DTO.AuthDTO;

/// <summary>
/// Data Transfer Object for password reset using verification code.
/// </summary>
public class ResetPasswordDTO
{
    /// <summary>
    /// User's email address.
    /// </summary>
    [Required(ErrorMessage = "El correo electrónico es obligatorio.")]
    [EmailAddress]
    public required string Email { get; set; }

    /// <summary>
    /// 6-digit verification code sent to the email address.
    /// </summary>
    [Required(ErrorMessage = "El código de verificación es obligatorio.")]
    [RegularExpression(@"^\d{6}$", ErrorMessage = "El código debe tener 6 dígitos.")]
    public required string Code { get; set; }

    /// <summary>
    /// New password the user wants to set. Must meet security requirements.
    /// </summary>
    [Required(ErrorMessage = "La nueva contraseña es obligatoria.")]
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

    /// <summary>
    /// Password confirmation. Must match the NewPassword field.
    /// </summary>
    [Required(ErrorMessage = "La confirmación de la contraseña es obligatoria.")]
    [Compare("NewPassword", ErrorMessage = "Las contraseñas no coinciden.")]
    public required string ConfirmPassword { get; set; }
}
