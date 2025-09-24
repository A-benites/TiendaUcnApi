using System.ComponentModel.DataAnnotations;

namespace TiendaUcnApi.src.Application.DTO.AuthDTO;

public class VerifyEmailChangeDTO
{
    [Required(ErrorMessage = "El nuevo correo electrónico es obligatorio.")]
    [EmailAddress(ErrorMessage = "El correo electrónico no tiene un formato válido.")]
    public required string NewEmail { get; set; }

    [Required(ErrorMessage = "El código de verificación es obligatorio.")]
    [RegularExpression(@"^\d{6}$", ErrorMessage = "El código debe tener 6 dígitos.")]
    public required string Code { get; set; }
}