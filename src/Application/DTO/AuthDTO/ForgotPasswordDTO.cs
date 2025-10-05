using System.ComponentModel.DataAnnotations;

/// <summary>
/// DTO para solicitar el restablecimiento de contraseña.
/// </summary>
public class ForgotPasswordDTO
{
    /// <summary>
    /// Correo electrónico del usuario.
    /// </summary>
    [EmailAddress]
    public required string Email { get; set; }
}