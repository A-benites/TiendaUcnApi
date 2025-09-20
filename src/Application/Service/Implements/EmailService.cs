using TiendaUcnApi.src.Application.Services.Interfaces;
using TiendaUcnApi.src.Domain.Models;

namespace TiendaUcnApi.src.Application.Services.Implements;

// Implementación simulada del servicio de correos.
// En una aplicación real, aquí iría la lógica para conectarse a un proveedor como Resend.
public class EmailService : IEmailService
{
    public Task SendVerificationCodeAsync(User user, string code)
    {
        // Por ahora, solo escribimos en la consola para simular el envío.
        Console.WriteLine("----- SIMULACIÓN DE ENVÍO DE CORREO -----");
        Console.WriteLine($"Para: {user.Email}");
        Console.WriteLine($"Asunto: ¡Bienvenido a Tienda UCN! Confirma tu cuenta.");
        Console.WriteLine($"Hola {user.FirstName}, tu código de verificación es: {code}");
        Console.WriteLine("-----------------------------------------");

        return Task.CompletedTask;
    }
}