using Resend;
using TiendaUcnApi.src.Application.Services.Interfaces;
using TiendaUcnApi.src.Domain.Models;

namespace TiendaUcnApi.src.Application.Services.Implements;

/// <summary>
/// Service for sending email messages using Resend API.
/// Supports template-based emails with placeholder replacements.
/// </summary>
public class EmailService : IEmailService
{
    /// <summary>
    /// Resend client for sending emails.
    /// </summary>
    private readonly IResend _resend;

    /// <summary>
    /// Application configuration.
    /// </summary>
    private readonly IConfiguration _configuration;

    /// <summary>
    /// Web hosting environment.
    /// </summary>
    private readonly IWebHostEnvironment _webHostEnvironment;

    /// <summary>
    /// Initializes a new instance of the EmailService class with all necessary dependencies.
    /// </summary>
    /// <param name="resend">Resend client for email delivery.</param>
    /// <param name="configuration">Application configuration.</param>
    /// <param name="webHostEnvironment">Web hosting environment for template path resolution.</param>
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
    /// Sends a verification code email to the user.
    /// Uses the VerificationCode email template.
    /// </summary>
    /// <param name="email">User's email address.</param>
    /// <param name="code">Verification code to send.</param>
    public async Task SendVerificationCodeEmailAsync(string email, string code)
    {
        var replacements = new Dictionary<string, string> { { "{{CODE}}", code } };
        var htmlBody = await LoadTemplate("VerificationCode", replacements);

        var message = new EmailMessage
        {
            To = email,
            Subject =
                _configuration["EmailConfiguration:VerificationSubject"] ?? "Verification Code",
            From =
                _configuration["EmailConfiguration:From"]
                ?? throw new InvalidOperationException("The 'From' configuration cannot be null."),
            HtmlBody = htmlBody,
        };
        await _resend.EmailSendAsync(message);
    }

    /// <summary>
    /// Sends a welcome email to the user after successful registration.
    /// Uses the Welcome email template.
    /// </summary>
    /// <param name="email">User's email address.</param>
    public async Task SendWelcomeEmailAsync(string email)
    {
        var htmlBody = await LoadTemplate("Welcome", null);

        var message = new EmailMessage
        {
            To = email,
            Subject = _configuration["EmailConfiguration:WelcomeSubject"] ?? "Welcome!",
            From =
                _configuration["EmailConfiguration:From"]
                ?? throw new InvalidOperationException("The 'From' configuration cannot be null."),
            HtmlBody = htmlBody,
        };

        await _resend.EmailSendAsync(message);
    }

    /// <summary>
    /// Sends a password reset email with a reset link.
    /// Uses the PasswordReset email template.
    /// </summary>
    /// <param name="to">Destination email address.</param>
    /// <param name="userName">User's name for personalization.</param>
    /// <param name="resetLink">Link to reset the password.</param>
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
            Subject = _configuration["EmailConfiguration:PasswordResetSubject"] ?? "Password Reset",
            From =
                _configuration["EmailConfiguration:From"]
                ?? throw new InvalidOperationException("The 'From' configuration cannot be null."),
            HtmlBody = htmlBody,
        };
        await _resend.EmailSendAsync(message);
    }

    /// <summary>
    /// Sends a verification code for password reset.
    /// Uses the PasswordResetCode email template.
    /// </summary>
    /// <param name="to">Destination email address.</param>
    /// <param name="userName">User's name for personalization.</param>
    /// <param name="code">Verification code for password reset.</param>
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
                ?? "Code to Reset Your Password",
            From =
                _configuration["EmailConfiguration:From"]
                ?? throw new InvalidOperationException("The 'From' configuration cannot be null."),
            HtmlBody = htmlBody,
        };
        await _resend.EmailSendAsync(message);
    }

    /// <summary>
    /// Loads an email template and replaces placeholders with actual values.
    /// Templates are located in src/Application/Templates/Email/.
    /// </summary>
    /// <param name="templateName">Template name without extension.</param>
    /// <param name="replacements">Dictionary of placeholders and their replacement values.</param>
    /// <returns>The HTML content of the template with values replaced.</returns>
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

    /// <summary>
    /// Sends an abandoned cart reminder email to encourage the user to complete their purchase.
    /// Uses the AbandonedCartReminder email template.
    /// </summary>
    /// <param name="toEmail">User's email address.</param>
    /// <param name="userName">User's name for personalization.</param>
    /// <param name="cartItemsHtml">HTML representation of cart items.</param>
    /// <param name="cartUrl">URL to the user's cart.</param>
    public async Task SendAbandonedCartReminderAsync(
        string toEmail,
        string userName,
        string cartItemsHtml,
        string cartUrl
    )
    {
        // Get the project root directory (navigate up from bin/Debug/netX.X)
        string baseDir = AppDomain.CurrentDomain.BaseDirectory;
        string? projectRoot = Directory.GetParent(baseDir)?.Parent?.Parent?.Parent?.FullName;

        if (projectRoot == null)
            throw new DirectoryNotFoundException("Could not determine the project path.");

        // Build the correct path to the template
        string templatePath = Path.Combine(
            projectRoot,
            "src",
            "Application",
            "Templates",
            "Email",
            "AbandonedCartReminder.html"
        );

        if (!File.Exists(templatePath))
            throw new FileNotFoundException($"Template not found at: {templatePath}");

        // Read the HTML file and replace variables
        string template = await File.ReadAllTextAsync(templatePath);
        string body = template
            .Replace("{{UserName}}", userName)
            .Replace("{{CartItems}}", cartItemsHtml)
            .Replace("{{CartUrl}}", cartUrl);

        // Send the email using Resend
        var message = new EmailMessage
        {
            From = "TiendaUCN <onboarding@resend.dev>",
            To = toEmail,
            Subject = "You still have products in your cart! 🛒",
            HtmlBody = body,
        };

        await _resend.EmailSendAsync(message);
    }
}
