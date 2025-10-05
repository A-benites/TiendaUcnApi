using TiendaUcnApi.src.Domain.Models;

namespace TiendaUcnApi.src.Application.Services.Interfaces;

/// <summary>
/// Interfaz para el servicio de envío de correos electrónicos.
/// </summary>
public interface IEmailService
{
    /// <summary>
    /// Envía un código de verificación al correo electrónico del usuario.
    /// </summary>
    /// <param name="email">El correo electrónico del usuario.</param>
    /// <param name="code">El código de verificación a enviar.</param>
    Task SendVerificationCodeEmailAsync(string email, string code);

    /// <summary>
    /// Envía un correo electrónico de bienvenida al usuario.
    /// </summary>
    /// <param name="email">El correo electrónico del usuario.</param>
    Task SendWelcomeEmailAsync(string email);

    Task SendPasswordResetEmailAsync(string to, string userName, string resetLink);

    /// <summary>
    /// Envía un código para restablecer la contraseña.
    /// </summary>
    /// <param name="to">El correo electrónico del destinatario.</param>
    /// <param name="userName">El nombre del usuario.</param>
    /// <param name="code">El código de 6 dígitos.</param>
    Task SendPasswordResetCodeEmailAsync(string to, string userName, string code);
}