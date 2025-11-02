using System.ComponentModel.DataAnnotations;

namespace TiendaUcnApi.src.Application.Validators;

/// <summary>
/// Custom validation attribute for birth date validation.
/// Ensures the birth date meets age requirements and is within reasonable bounds.
/// </summary>
public class BirthDateValidationAttribute : ValidationAttribute
{
    /// <summary>
    /// Validates that the birth date is not in the future,
    /// the person is at least 18 years old, and not older than 120 years.
    /// </summary>
    /// <param name="value">The birth date to validate.</param>
    /// <param name="validationContext">The context in which the validation is performed.</param>
    /// <returns>ValidationResult indicating success or failure with error message.</returns>
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        // Working with DateTime type is safer and more direct
        if (value is not DateTime birthDate)
        {
            // If value is not a date, assume success,
            // as [Required] and model binding handle nulls and formats.
            return ValidationResult.Success;
        }

        if (birthDate > DateTime.Today)
        {
            return new ValidationResult("La fecha de nacimiento no puede ser futura.");
        }

        // Using AddYears is more precise for calculating exact age
        if (birthDate > DateTime.Today.AddYears(-18))
        {
            return new ValidationResult("Debes ser mayor de 18 años.");
        }

        if (birthDate < DateTime.Today.AddYears(-120))
        {
            return new ValidationResult("La fecha de nacimiento no puede ser mayor a 120 años.");
        }

        return ValidationResult.Success;
    }
}
