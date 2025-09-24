using System.ComponentModel.DataAnnotations;
using TiendaUcnApi.src.Application.Validators;

namespace TiendaUcnApi.src.Application.DTO;

public class UpdateProfileDTO
{
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

    [RegularExpression(
        @"^(Masculino|Femenino|Otro)$",
        ErrorMessage = "El género debe ser Masculino, Femenino u Otro."
    )]
    public string? Gender { get; set; }

    [BirthDateValidation]
    public DateOnly? BirthDate { get; set; }

    [RutValidation(ErrorMessage = "El RUT no es válido.")]
    public string? Rut { get; set; }

    [EmailAddress(ErrorMessage = "El email no tiene un formato válido.")]
    public string? Email { get; set; }

    [RegularExpression(
        @"^\+569\d{8}$",
        ErrorMessage = "El formato del teléfono debe ser +569xxxxxxxx."
    )]
    public string? PhoneNumber { get; set; }
}