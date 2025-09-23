using System.ComponentModel.DataAnnotations;

namespace TiendaUcnApi.src.Application.Validators;

public class BirthDateValidationAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        // Es más seguro y directo trabajar con el tipo DateTime
        if (value is not DateTime birthDate)
        {
            // Si el valor no es una fecha, se asume éxito,
            // ya que [Required] y el model binding se encargan de los nulos y formatos.
            return ValidationResult.Success;
        }

        if (birthDate > DateTime.Today)
        {
            return new ValidationResult("La fecha de nacimiento no puede ser futura.");
        }

        // Usar AddYears es más preciso para calcular la edad exacta.
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