using System.ComponentModel.DataAnnotations;
using TiendaUcnApi.src.Application.Validators;

namespace TiendaUcnApi.src.Application.DTO;

/// <summary>
/// Data Transfer Object for updating user profile data.
/// </summary>
public class UpdateProfileDTO
{
    /// <summary>
    /// User's first name (optional). Must contain only letters, spaces, and hyphens.
    /// </summary>
    [StringLength(
        50,
        MinimumLength = 2,
        ErrorMessage = "El nombre debe tener entre 2 y 50 caracteres."
    )]
    [RegularExpression(
        @"^[a-zA-ZáéíóúÁÉÍÓÚüÜñÑ\s\-]+$",
        ErrorMessage = "El nombre solo puede contener letras, espacios y guiones."
    )]
    public string? FirstName { get; set; }

    /// <summary>
    /// User's last name (optional). Must contain only letters, spaces, and hyphens.
    /// </summary>
    [StringLength(
        50,
        MinimumLength = 2,
        ErrorMessage = "El apellido debe tener entre 2 y 50 caracteres."
    )]
    [RegularExpression(
        @"^[a-zA-ZáéíóúÁÉÍÓÚüÜñÑ\s\-]+$",
        ErrorMessage = "El apellido solo puede contener letras, espacios y guiones."
    )]
    public string? LastName { get; set; }

    /// <summary>
    /// User's gender (optional). Must be Masculino, Femenino, or Otro.
    /// </summary>
    [RegularExpression(
        @"^(Masculino|Femenino|Otro)$",
        ErrorMessage = "El género debe ser Masculino, Femenino u Otro."
    )]
    public string? Gender { get; set; }

    /// <summary>
    /// User's birth date (optional). Must meet age validation requirements.
    /// </summary>
    [BirthDateValidation]
    public DateOnly? BirthDate { get; set; }

    /// <summary>
    /// Chilean national identification number - RUT (optional). Must be in valid format.
    /// </summary>
    [RutValidation(ErrorMessage = "El RUT no es válido.")]
    public string? Rut { get; set; }

    /// <summary>
    /// User's email address (optional). Must be in valid email format.
    /// </summary>
    [EmailAddress(ErrorMessage = "El email no tiene un formato válido.")]
    public string? Email { get; set; }

    /// <summary>
    /// User's phone number (optional). Must be in Chilean format (+569xxxxxxxx).
    /// </summary>
    [RegularExpression(
        @"^\+569\d{8}$",
        ErrorMessage = "El formato del teléfono debe ser +569xxxxxxxx."
    )]
    public string? PhoneNumber { get; set; }
}
