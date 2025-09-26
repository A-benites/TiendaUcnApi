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
    /// </summary>
    [Required(ErrorMessage = "La nueva contraseña es requerida.")]
    public required string NewPassword { get; set; } = string.Empty;
}