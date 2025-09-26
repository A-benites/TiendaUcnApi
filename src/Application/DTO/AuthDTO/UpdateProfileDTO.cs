using System.ComponentModel.DataAnnotations;
using TiendaUcnApi.src.Application.Validators;

namespace TiendaUcnApi.src.Application.DTO;

/// <summary>
/// DTO para la actualización de datos del perfil de usuario.
/// </summary>
public class UpdateProfileDTO
{
    /// <summary>
    /// Primer nombre del usuario.
    /// </summary>
    [StringLength(50, MinimumLength = 2, ErrorMessage = "El nombre debe tener entre 2 y 50 caracteres.")]
    [RegularExpression(@"^[a-zA-ZáéíóúÁÉÍÓÚüÜñÑ\s\-]+$", ErrorMessage = "El nombre solo puede contener letras, espacios y guiones.")]
    public string? FirstName { get; set; }

    /// <summary>
    /// Apellido del usuario.
    /// </summary>
    [StringLength(50, MinimumLength = 2, ErrorMessage = "El apellido debe tener entre 2 y 50 caracteres.")]
    [RegularExpression(@"^[a-zA-ZáéíóúÁÉÍÓÚüÜñÑ\s\-]+$", ErrorMessage = "El apellido solo puede contener letras, espacios y guiones.")]
    public string? LastName { get; set; }

    /// <summary>
    /// Género del usuario (Masculino, Femenino u Otro).
    /// </summary>
    [RegularExpression(@"^(Masculino|Femenino|Otro)$", ErrorMessage = "El género debe ser Masculino, Femenino u Otro.")]
    public string? Gender { get; set; }

    /// <summary>
    /// Fecha de nacimiento del usuario.
    /// </summary>
    [BirthDateValidation]
    public DateOnly? BirthDate { get; set; }

    /// <summary>
    /// RUT del usuario.
    /// </summary>
    [RutValidation(ErrorMessage = "El RUT no es válido.")]
    public string? Rut { get; set; }

    /// <summary>
    /// Correo electrónico del usuario.
    /// </summary>
    [EmailAddress(ErrorMessage = "El email no tiene un formato válido.")]
    public string? Email { get; set; }

    /// <summary>
    /// Número de teléfono del usuario en formato +569xxxxxxxx.
    /// </summary>
    [RegularExpression(@"^\+569\d{8}$", ErrorMessage = "El formato del teléfono debe ser +569xxxxxxxx.")]
    public string? PhoneNumber { get; set; }
}