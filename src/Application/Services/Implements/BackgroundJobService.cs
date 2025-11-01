using TiendaUcnApi.src.Application.Services.Interfaces;
using TiendaUcnApi.src.Infrastructure.Repositories.Interfaces;
using TiendaUcnApi.src.Domain.Models;
using Serilog;

namespace TiendaUcnApi.src.Application.Services.Implements
{
    /// <summary>
    /// Handles recurring background jobs executed by Hangfire.
    /// Includes tasks for deleting unverified users and sending abandoned cart reminders.
    /// </summary>
    public class BackgroundJobService : IBackgroundJobService
    {
        private readonly IUserRepository _userRepository;
        private readonly ICartRepository _cartRepository;
        private readonly IEmailService _emailService;

        public BackgroundJobService(
            IUserRepository userRepository,
            ICartRepository cartRepository,
            IEmailService emailService
        )
        {
            _userRepository = userRepository;
            _cartRepository = cartRepository;
            _emailService = emailService;
        }

        /// <summary>
        /// Deletes users who haven't confirmed their email accounts within a set number of days.
        /// </summary>
        public async Task DeleteUnconfirmedUsersAsync()
        {
            try
            {
                var unverifiedUsers = await _userRepository.GetUnconfirmedUsersAsync();

                foreach (var user in unverifiedUsers)
                {
                    await _userRepository.DeleteAsync(user.Id);
                    Log.Information("Deleted unverified user: {Email}", user.Email);
                }

                Log.Information("Job completed: deleted {Count} unverified users", unverifiedUsers.Count);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error while deleting unverified users.");
                throw;
            }
        }

        /// <summary>
        /// Sends reminder emails for abandoned carts (not updated in the last 3 days).
        /// </summary>
        public async Task SendAbandonedCartRemindersAsync()
        {
            try
            {
                // 🔹 Trae directamente los carritos abandonados desde el repositorio
                var abandonedCarts = await _cartRepository.GetAbandonedCartsAsync();

                Log.Information("Job started: found {Count} abandoned carts", abandonedCarts.Count);

                foreach (var cart in abandonedCarts)
                {
                    if (cart.User?.Email == null)
                    {
                        Log.Warning("Skipping cart {Id} because user email is null", cart.Id);
                        continue;
                    }

                    if (cart.CartItems == null || cart.CartItems.Count == 0)
                    {
                        Log.Warning("Skipping cart {Id} because it has no items", cart.Id);
                        continue;
                    }

                    // 🔹 Genera una lista HTML de productos
                    var cartItemsList = new List<string>();
                    foreach (var item in cart.CartItems)
                    {
                        var title = item.Product?.Title ?? "Producto desconocido";
                        cartItemsList.Add($"• {title} - {item.Quantity} unidades");
                    }

                    var cartSummary = string.Join("<br>", cartItemsList);

                    // 🔹 Enviar correo recordatorio
                    await _emailService.SendAbandonedCartReminderAsync(
                        cart.User.Email,
                        cart.User.FirstName ?? "Cliente",
                        cartSummary,
                        "https://tienda-ucn.cl/cart"
                    );

                    Log.Information("Sent abandoned cart reminder to: {Email}", cart.User.Email);
                }

                Log.Information("Job completed: reminders sent for {Count} abandoned carts", abandonedCarts.Count);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error while sending abandoned cart reminders.");
                throw;
            }
        }
    }
}
