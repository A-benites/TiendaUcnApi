using System.ComponentModel.DataAnnotations;

namespace TiendaUcnApi.src.Application.DTO.AuthDTO;

/// <summary>
/// DTO para la verificación del cambio de correo electrónico.
/// </summary>
public class VerifyEmailChangeDTO
{
    /// <summary>
    /// Nuevo correo electrónico que se desea establecer.
    /// </summary>
    [Required(ErrorMessage = "El nuevo correo electrónico es obligatorio.")]
    [EmailAddress(ErrorMessage = "El correo electrónico no tiene un formato válido.")]
    public required string NewEmail { get; set; }

    /// <summary>
    /// Código de verificación enviado al nuevo correo electrónico.
    /// </summary>
    [Required(ErrorMessage = "El código de verificación es obligatorio.")]
    [RegularExpression(@"^\d{6}$", ErrorMessage = "El código debe tener 6 dígitos.")]
    public required string Code { get; set; }
}