using TiendaUcnApi.src.Domain.Models;

namespace TiendaUcnApi.src.Application.Services.Interfaces;

// Define el contrato para cualquier servicio de envío de correos.
public interface IEmailService
{
    /// <summary>
    /// Envía un correo electrónico con el código de verificación al usuario.
    /// </summary>
    /// <param name="user">El usuario destinatario.</param>
    /// <param name="code">El código de verificación a enviar.</param>
    Task SendVerificationCodeAsync(User user, string code);
}