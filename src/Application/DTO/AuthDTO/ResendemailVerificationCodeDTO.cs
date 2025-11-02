using System.ComponentModel.DataAnnotations;

namespace TiendaUcnApi.src.Application.DTO.AuthDTO;

/// <summary>
/// Data Transfer Object for requesting to resend email verification code.
/// </summary>
public class ResendEmailVerificationCodeDTO
{
    /// <summary>
    /// User's email address where the verification code will be resent.
    /// </summary>
    [Required(ErrorMessage = "El correo electrónico es obligatorio.")]
    [EmailAddress(ErrorMessage = "El correo electrónico no tiene un formato válido.")]
    public required string Email { get; set; }
}
