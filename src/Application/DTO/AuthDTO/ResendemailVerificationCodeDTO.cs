using System.ComponentModel.DataAnnotations;

namespace TiendaUcnApi.src.Application.DTO.AuthDTO;

/// <summary>
/// DTO para solicitar el reenvío del código de verificación al correo electrónico.
/// </summary>
public class ResendEmailVerificationCodeDTO
{
    /// <summary>
    /// Correo electrónico del usuario.
    /// </summary>
    [Required(ErrorMessage = "El correo electrónico es obligatorio.")]
    [EmailAddress(ErrorMessage = "El correo electrónico no tiene un formato válido.")]
    public required string Email { get; set; }
}