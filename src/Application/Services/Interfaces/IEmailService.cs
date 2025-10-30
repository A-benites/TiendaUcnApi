using TiendaUcnApi.src.Domain.Models;

namespace TiendaUcnApi.src.Application.Services.Interfaces;

public interface IEmailService
{
    Task SendVerificationCodeEmailAsync(string email, string code);
    Task SendWelcomeEmailAsync(string email);
    Task SendPasswordResetEmailAsync(string to, string userName, string resetLink);
    Task SendPasswordResetCodeEmailAsync(string to, string userName, string code);

    // New method for Hangfire cart reminder
    Task SendAbandonedCartReminderAsync(string to, string cartSummary);
}
