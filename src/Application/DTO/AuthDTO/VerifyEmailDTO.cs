using System.ComponentModel.DataAnnotations;

namespace TiendaUcnApi.src.Application.DTO.AuthDTO;

/// <summary>
/// Data Transfer Object for user email verification.
/// </summary>
public class VerifyEmailDTO
{
    /// <summary>
    /// User's email address to be verified.
    /// </summary>
    [Required(ErrorMessage = "El correo electrónico es obligatorio.")]
    [EmailAddress(ErrorMessage = "El correo electrónico no tiene un formato válido.")]
    public required string Email { get; set; }

    /// <summary>
    /// 6-digit verification code sent to the email address.
    /// </summary>
    [Required(ErrorMessage = "El código de verificación es obligatorio.")]
    [RegularExpression(
        @"^\d{6}$",
        ErrorMessage = "El código de verificación debe tener 6 dígitos."
    )]
    public required string VerificationCode { get; set; }
}
