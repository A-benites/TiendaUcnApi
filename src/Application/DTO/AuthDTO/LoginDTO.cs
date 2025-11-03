using System.ComponentModel.DataAnnotations;

/// <summary>
/// Data Transfer Object for user login.
/// </summary>
public class LoginDTO
{
    /// <summary>
    /// User's email address.
    /// </summary>
    [Required(ErrorMessage = "El email es requerido.")]
    [EmailAddress(ErrorMessage = "El Correo electrónico no es válido.")]
    public required string Email { get; set; }

    /// <summary>
    /// User's password.
    /// </summary>
    [Required(ErrorMessage = "La contraseña es requerida.")]
    public required string Password { get; set; }

    /// <summary>
    /// Indicates whether the user wants to remember the session.
    /// </summary>
    public bool RememberMe { get; set; }
}
