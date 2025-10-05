using System.ComponentModel.DataAnnotations;

/// <summary>
/// DTO para el inicio de sesión de usuario.
/// </summary>
public class LoginDTO
{
    /// <summary>
    /// Correo electrónico del usuario.
    /// </summary>
    [Required(ErrorMessage = "El email es requerido.")]
    [EmailAddress(ErrorMessage = "El Correo electrónico no es válido.")]
    public required string Email { get; set; }

    /// <summary>
    /// Contraseña del usuario.
    /// </summary>
    [Required(ErrorMessage = "La contraseña es requerida.")]
    public required string Password { get; set; }

    /// <summary>
    /// Indica si el usuario desea recordar la sesión.
    /// </summary>
    public bool RememberMe { get; set; }
}