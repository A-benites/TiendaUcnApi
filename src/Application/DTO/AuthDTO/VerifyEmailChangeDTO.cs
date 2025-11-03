using System.ComponentModel.DataAnnotations;

namespace TiendaUcnApi.src.Application.DTO.AuthDTO;

/// <summary>
/// Data Transfer Object for email change verification.
/// </summary>
public class VerifyEmailChangeDTO
{
    /// <summary>
    /// New email address to be set for the user.
    /// </summary>
    [Required(ErrorMessage = "El nuevo correo electrónico es obligatorio.")]
    [EmailAddress(ErrorMessage = "El correo electrónico no tiene un formato válido.")]
    public required string NewEmail { get; set; }

    /// <summary>
    /// 6-digit verification code sent to the new email address.
    /// </summary>
    [Required(ErrorMessage = "El código de verificación es obligatorio.")]
    [RegularExpression(@"^\d{6}$", ErrorMessage = "El código debe tener 6 dígitos.")]
    public required string Code { get; set; }
}
