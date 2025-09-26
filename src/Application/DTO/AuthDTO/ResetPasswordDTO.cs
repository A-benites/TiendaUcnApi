using System.ComponentModel.DataAnnotations;

namespace TiendaUcnApi.src.Application.DTO.AuthDTO;

/// <summary>
/// DTO para el restablecimiento de contraseña mediante código de verificación.
/// </summary>
public class ResetPasswordDTO
{
    /// <summary>
    /// Correo electrónico del usuario.
    /// </summary>
    [Required(ErrorMessage = "El correo electrónico es obligatorio.")]
    [EmailAddress]
    public required string Email { get; set; }

    /// <summary>
    /// Código de verificación enviado al correo electrónico.
    /// </summary>
    [Required(ErrorMessage = "El código de verificación es obligatorio.")]
    [RegularExpression(@"^\d{6}$", ErrorMessage = "El código debe tener 6 dígitos.")]
    public required string Code { get; set; } // Cambiado de Token a Code

    /// <summary>
    /// Nueva contraseña que el usuario desea establecer.
    /// </summary>
    [Required(ErrorMessage = "La nueva contraseña es obligatoria.")]
    [StringLength(20, MinimumLength = 8, ErrorMessage = "La contraseña debe tener entre 8 y 20 caracteres.")]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$", ErrorMessage = "La contraseña debe tener al menos una mayúscula, una minúscula, un número y un carácter especial.")]
    public required string NewPassword { get; set; }

    /// <summary>
    /// Confirmación de la nueva contraseña.
    /// </summary>
    [Required(ErrorMessage = "La confirmación de la contraseña es obligatoria.")]
    [Compare("NewPassword", ErrorMessage = "Las contraseñas no coinciden.")]
    public required string ConfirmPassword { get; set; }
}