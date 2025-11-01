using Resend;
using TiendaUcnApi.src.Application.Services.Interfaces;
using TiendaUcnApi.src.Domain.Models;

namespace TiendaUcnApi.src.Application.Services.Implements;

/// <summary>
/// Servicio para enviar correos electrónicos.
/// </summary>
public class EmailService : IEmailService
{
    /// <summary>
    /// Cliente para enviar correos con Resend.
    /// </summary>
    private readonly IResend _resend;

    /// <summary>
    /// Configuración de la aplicación.
    /// </summary>
    private readonly IConfiguration _configuration;

    /// <summary>
    /// Entorno de hosting web.
    /// </summary>
    private readonly IWebHostEnvironment _webHostEnvironment;

    /// <summary>
    /// Constructor con todas las dependencias necesarias.
    /// </summary>
    /// <param name="resend">Cliente Resend.</param>
    /// <param name="configuration">Configuración de la aplicación.</param>
    /// <param name="webHostEnvironment">Entorno de hosting web.</param>
    public EmailService(
        IResend resend,
        IConfiguration configuration,
        IWebHostEnvironment webHostEnvironment
    )
    {
        _resend = resend;
        _configuration = configuration;
        _webHostEnvironment = webHostEnvironment;
    }

    /// <summary>
    /// Envía un código de verificación al correo electrónico del usuario.
    /// </summary>
    /// <param name="email">Correo electrónico del usuario.</param>
    /// <param name="code">Código de verificación.</param>
    public async Task SendVerificationCodeEmailAsync(string email, string code)
    {
        var replacements = new Dictionary<string, string> { { "{{CODE}}", code } };
        var htmlBody = await LoadTemplate("VerificationCode", replacements);

        var message = new EmailMessage
        {
            To = email,
            Subject =
                _configuration["EmailConfiguration:VerificationSubject"]
                ?? "Código de Verificación",
            From =
                _configuration["EmailConfiguration:From"]
                ?? throw new InvalidOperationException(
                    "La configuración de 'From' no puede ser nula."
                ),
            HtmlBody = htmlBody,
        };
        await _resend.EmailSendAsync(message);
    }

    /// <summary>
    /// Envía un correo electrónico de bienvenida al usuario.
    /// </summary>
    /// <param name="email">Correo electrónico del usuario.</param>
    public async Task SendWelcomeEmailAsync(string email)
    {
        var htmlBody = await LoadTemplate("Welcome", null);

        var message = new EmailMessage
        {
            To = email,
            Subject = _configuration["EmailConfiguration:WelcomeSubject"] ?? "¡Bienvenido/a!",
            From =
                _configuration["EmailConfiguration:From"]
                ?? throw new InvalidOperationException(
                    "La configuración de 'From' no puede ser nula."
                ),
            HtmlBody = htmlBody,
        };

        await _resend.EmailSendAsync(message);
    }

    /// <summary>
    /// Envía un correo para restablecer la contraseña.
    /// </summary>
    /// <param name="to">Correo electrónico de destino.</param>
    /// <param name="userName">Nombre del usuario.</param>
    /// <param name="resetLink">Enlace para restablecer la contraseña.</param>
    public async Task SendPasswordResetEmailAsync(string to, string userName, string resetLink)
    {
        var replacements = new Dictionary<string, string>
        {
            { "{{UserName}}", userName },
            { "{{ResetLink}}", resetLink },
        };
        var htmlBody = await LoadTemplate("PasswordReset", replacements);

        var message = new EmailMessage
        {
            To = to,
            Subject =
                _configuration["EmailConfiguration:PasswordResetSubject"]
                ?? "Restablecimiento de Contraseña",
            From =
                _configuration["EmailConfiguration:From"]
                ?? throw new InvalidOperationException(
                    "La configuración de 'From' no puede ser nula."
                ),
            HtmlBody = htmlBody,
        };
        await _resend.EmailSendAsync(message);
    }

    /// <summary>
    /// Envía un código para restablecer la contraseña.
    /// </summary>
    /// <param name="to">Correo electrónico de destino.</param>
    /// <param name="userName">Nombre del usuario.</param>
    /// <param name="code">Código de verificación.</param>
    public async Task SendPasswordResetCodeEmailAsync(string to, string userName, string code)
    {
        var replacements = new Dictionary<string, string>
        {
            { "{{UserName}}", userName },
            { "{{CODE}}", code },
        };
        var htmlBody = await LoadTemplate("PasswordResetCode", replacements);

        var message = new EmailMessage
        {
            To = to,
            Subject =
                _configuration["EmailConfiguration:PasswordResetSubject"]
                ?? "Código para Restablecer tu Contraseña",
            From =
                _configuration["EmailConfiguration:From"]
                ?? throw new InvalidOperationException(
                    "La configuración de 'From' no puede ser nula."
                ),
            HtmlBody = htmlBody,
        };
        await _resend.EmailSendAsync(message);
    }

    /// <summary>
    /// Carga una plantilla de correo electrónico y reemplaza los marcadores de posición.
    /// </summary>
    /// <param name="templateName">El nombre de la plantilla sin extensión.</param>
    /// <param name="replacements">Un diccionario con los marcadores de posición y sus valores.</param>
    /// <returns>El contenido HTML de la plantilla con los valores reemplazados.</returns>
    private async Task<string> LoadTemplate(
        string templateName,
        Dictionary<string, string>? replacements
    )
    {
        var templatePath = Path.Combine(
            _webHostEnvironment.ContentRootPath,
            "src",
            "Application",
            "Templates",
            "Email",
            $"{templateName}.html"
        );
        var html = await File.ReadAllTextAsync(templatePath);

        if (replacements != null)
        {
            foreach (var replacement in replacements)
            {
                html = html.Replace(replacement.Key, replacement.Value);
            }
        }

        return html;
    }

    public async Task SendAbandonedCartReminderAsync(string toEmail, string userName, string cartItemsHtml, string cartUrl)
    {
        // 🔹 Obtener el directorio raíz del proyecto (subimos desde bin/Debug/netX.X)
        string baseDir = AppDomain.CurrentDomain.BaseDirectory;
        string? projectRoot = Directory.GetParent(baseDir)?
            .Parent?.Parent?.Parent?.FullName;

        if (projectRoot == null)
            throw new DirectoryNotFoundException("No se pudo determinar la ruta del proyecto.");

        // 🔹 Construir la ruta correcta al template
        string templatePath = Path.Combine(projectRoot, "src", "Application", "Templates", "Email", "AbandonedCartReminder.html");

        if (!File.Exists(templatePath))
            throw new FileNotFoundException($"No se encontró la plantilla en: {templatePath}");

        // 🔹 Leer el archivo HTML y reemplazar variables
        string template = await File.ReadAllTextAsync(templatePath);
        string body = template
            .Replace("{{UserName}}", userName)
            .Replace("{{CartItems}}", cartItemsHtml)
            .Replace("{{CartUrl}}", cartUrl);

        // 🔹 Enviar el correo usando Resend
        var message = new EmailMessage
        {
            From = "TiendaUCN <onboarding@resend.dev>",
            To = toEmail,
            Subject = "¡Aún tienes productos en tu carrito! 🛒",
            HtmlBody = body
        };

        await _resend.EmailSendAsync(message);
    }


}