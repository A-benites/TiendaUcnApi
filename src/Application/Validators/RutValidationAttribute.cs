using System.ComponentModel.DataAnnotations;

namespace TiendaUcnApi.src.Application.Validators;

public class RutValidationAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is not string rut || string.IsNullOrWhiteSpace(rut))
        {
            return ValidationResult.Success;
        }

        try
        {
            rut = rut.ToUpper().Replace(".", "").Replace("-", "");
            if (rut.Length < 2)
                return new ValidationResult("El RUT no es válido.");

            char dv = rut[^1]; // Obtiene el último caracter (dígito verificador)
            string rutBody = rut.Substring(0, rut.Length - 1);

            // Asegurarse de que el cuerpo del RUT es numérico
            if (!long.TryParse(rutBody, out long rutNumber))
            {
                return new ValidationResult("El cuerpo del RUT debe ser numérico.");
            }

            // Cálculo del dígito verificador
            int suma = 0;
            int multiplicador = 2;
            for (int i = rutBody.Length - 1; i >= 0; i--)
            {
                suma += int.Parse(rutBody[i].ToString()) * multiplicador;
                multiplicador = multiplicador == 7 ? 2 : multiplicador + 1;
            }

            int resto = 11 - (suma % 11);
            char dvCalculado;

            if (resto == 11)
            {
                dvCalculado = '0';
            }
            else if (resto == 10)
            {
                dvCalculado = 'K';
            }
            else
            {
                dvCalculado = resto.ToString()[0];
            }

            if (dv == dvCalculado)
            {
                return ValidationResult.Success;
            }
            else
            {
                return new ValidationResult("El RUT ingresado no es válido.");
            }
        }
        catch (System.Exception)
        {
            return new ValidationResult("El formato del RUT no es válido.");
        }
    }
}