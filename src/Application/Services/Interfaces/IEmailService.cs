using TiendaUcnApi.src.Domain.Models;

namespace TiendaUcnApi.src.Application.Services.Interfaces;

/// <summary>
/// Service interface for email operations using Resend API.
/// </summary>
public interface IEmailService
{
    /// <summary>
    /// Sends an email verification code to the user's email address.
    /// </summary>
    /// <param name="email">The recipient's email address.</param>
    /// <param name="code">The verification code to send.</param>
    Task SendVerificationCodeEmailAsync(string email, string code);

    /// <summary>
    /// Sends a welcome email to a newly registered user.
    /// </summary>
    /// <param name="email">The new user's email address.</param>
    Task SendWelcomeEmailAsync(string email);

    /// <summary>
    /// Sends a password reset email with a reset link.
    /// </summary>
    /// <param name="to">The recipient's email address.</param>
    /// <param name="userName">The user's name for personalization.</param>
    /// <param name="resetLink">The password reset link.</param>
    Task SendPasswordResetEmailAsync(string to, string userName, string resetLink);

    /// <summary>
    /// Sends a password reset email with a verification code.
    /// </summary>
    /// <param name="to">The recipient's email address.</param>
    /// <param name="userName">The user's name for personalization.</param>
    /// <param name="code">The password reset code.</param>
    Task SendPasswordResetCodeEmailAsync(string to, string userName, string code);

    /// <summary>
    /// Sends an abandoned cart reminder email to the user.
    /// Background job method used by Hangfire.
    /// </summary>
    /// <param name="toEmail">The recipient's email address.</param>
    /// <param name="userName">The user's name for personalization.</param>
    /// <param name="cartItemsHtml">HTML content describing the cart items.</param>
    /// <param name="cartUrl">URL to view/complete the cart.</param>
    Task SendAbandonedCartReminderAsync(
        string toEmail,
        string userName,
        string cartItemsHtml,
        string cartUrl
    );
}
