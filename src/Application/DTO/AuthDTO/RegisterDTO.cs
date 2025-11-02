using System.ComponentModel.DataAnnotations;
using TiendaUcnApi.src.Application.Validators;

namespace TiendaUcnApi.src.Application.DTO.AuthDTO;

/// <summary>
/// Data Transfer Object for new user registration.
/// </summary>
public class RegisterDTO
{
    /// <summary>
    /// User's first name. Must contain only letters, spaces, and hyphens.
    /// </summary>
    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [StringLength(
        50,
        MinimumLength = 2,
        ErrorMessage = "El nombre debe tener entre 2 y 50 caracteres."
    )]
    [RegularExpression(
        @"^[a-zA-ZáéíóúÁÉÍÓÚüÜñÑ\s\-]+$",
        ErrorMessage = "El nombre solo puede contener letras, espacios y guiones."
    )]
    public required string FirstName { get; set; }

    /// <summary>
    /// User's last name. Must contain only letters, spaces, and hyphens.
    /// </summary>
    [Required(ErrorMessage = "El apellido es obligatorio.")]
    [StringLength(
        50,
        MinimumLength = 2,
        ErrorMessage = "El apellido debe tener entre 2 y 50 caracteres."
    )]
    [RegularExpression(
        @"^[a-zA-ZáéíóúÁÉÍÓÚüÜñÑ\s\-]+$",
        ErrorMessage = "El apellido solo puede contener letras, espacios y guiones."
    )]
    public required string LastName { get; set; }

    /// <summary>
    /// User's gender (Masculino, Femenino, or Otro).
    /// </summary>
    [Required(ErrorMessage = "El género es obligatorio.")]
    [RegularExpression(
        @"^(Masculino|Femenino|Otro)$",
        ErrorMessage = "El género debe ser Masculino, Femenino u Otro."
    )]
    public required string Gender { get; set; }

    /// <summary>
    /// User's birth date. Must be validated to ensure the user meets age requirements.
    /// </summary>
    [Required(ErrorMessage = "La fecha de nacimiento es obligatoria.")]
    [BirthDateValidation]
    public required DateOnly BirthDate { get; set; }

    /// <summary>
    /// Chilean national identification number (RUT). Must be in valid format and verified.
    /// </summary>
    [Required(ErrorMessage = "El RUT es obligatorio.")]
    [RutValidation(ErrorMessage = "El RUT no es válido.")]
    public required string Rut { get; set; }

    /// <summary>
    /// User's email address. Must be a valid email format.
    /// </summary>
    [Required(ErrorMessage = "El email es obligatorio.")]
    [EmailAddress(ErrorMessage = "El email no tiene un formato válido.")]
    public required string Email { get; set; }

    /// <summary>
    /// User's phone number in Chilean format (+569xxxxxxxx).
    /// </summary>
    [Required(ErrorMessage = "El número de teléfono es obligatorio.")]
    [RegularExpression(
        @"^\+569\d{8}$",
        ErrorMessage = "El formato del teléfono debe ser +569xxxxxxxx."
    )]
    public required string PhoneNumber { get; set; }

    /// <summary>
    /// User's password. Must contain at least one uppercase letter, one lowercase letter, one digit, and one special character.
    /// </summary>
    [Required(ErrorMessage = "La contraseña es requerida.")]
    [StringLength(
        20,
        MinimumLength = 8,
        ErrorMessage = "La contraseña debe tener entre 8 y 20 caracteres."
    )]
    [RegularExpression(
        @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$",
        ErrorMessage = "La contraseña debe tener al menos una mayúscula, una minúscula, un número y un carácter especial."
    )]
    public required string Password { get; set; }

    /// <summary>
    /// Password confirmation. Must match the Password field.
    /// </summary>
    [Required(ErrorMessage = "La confirmación de la contraseña es obligatoria.")]
    [Compare("Password", ErrorMessage = "Las contraseñas no coinciden.")]
    public required string ConfirmPassword { get; set; }
}
