using System.ComponentModel.DataAnnotations;
using TiendaUcnApi.src.Application.Validators;

namespace TiendaUcnApi.src.Application.DTO.AuthDTO;

public class RegisterDTO
{
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

    [Required(ErrorMessage = "El apellido es obligatorio.")]
    [StringLength(
        50,
        MinimumLength = 2,
        ErrorMessage = "El apellido debe tener entre 2 y 50 caracteres."
    )] // AJUSTE: MaxLength a 50
    [RegularExpression(
        @"^[a-zA-ZáéíóúÁÉÍÓÚüÜñÑ\s\-]+$",
        ErrorMessage = "El apellido solo puede contener letras, espacios y guiones."
    )]
    public required string LastName { get; set; }

    [Required(ErrorMessage = "El género es obligatorio.")]
    [RegularExpression(
        @"^(Masculino|Femenino|Otro)$",
        ErrorMessage = "El género debe ser Masculino, Femenino u Otro."
    )]
    public required string Gender { get; set; }

    [Required(ErrorMessage = "La fecha de nacimiento es obligatoria.")]
    [BirthDateValidation]
    public required DateOnly BirthDate { get; set; }

    [Required(ErrorMessage = "El RUT es obligatorio.")]
    [RutValidation(ErrorMessage = "El RUT no es válido.")]
    public required string Rut { get; set; }

    [Required(ErrorMessage = "El email es obligatorio.")]
    [EmailAddress(ErrorMessage = "El email no tiene un formato válido.")]
    public required string Email { get; set; }

    [Required(ErrorMessage = "El número de teléfono es obligatorio.")]
    [RegularExpression(
        @"^\+569\d{8}$",
        ErrorMessage = "El formato del teléfono debe ser +569xxxxxxxx."
    )] // AJUSTE: Formato estándar
    public required string PhoneNumber { get; set; }

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

    [Required(ErrorMessage = "La confirmación de la contraseña es obligatoria.")]
    [Compare("Password", ErrorMessage = "Las contraseñas no coinciden.")]
    public required string ConfirmPassword { get; set; }
}