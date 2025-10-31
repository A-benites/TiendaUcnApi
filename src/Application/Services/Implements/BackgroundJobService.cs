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
                // Retrieve all unverified users
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
                var abandonedCarts = await _cartRepository.GetAllAsync();

                // Filter carts that have not been updated in the last 3 days
                var oldCarts = abandonedCarts
                    .Where(c => (DateTime.UtcNow - c.UpdatedAt).TotalDays >= 3)
                    .ToList();

                foreach (var cart in oldCarts)
                {
                    if (cart.User?.Email == null)
                        continue;

                    // Generate a simple HTML list of cart items
                    var cartItemsList = new List<string>();
                    foreach (var item in cart.CartItems)
                    {
                        // Safely access Product information
                        var title = item.Product?.Title ?? "Unknown product";
                        cartItemsList.Add($"• {title} - {item.Quantity} units");
                    }

                    var cartSummary = string.Join("<br>", cartItemsList);

                    await _emailService.SendAbandonedCartReminderAsync(
                        cart.User.Email,
                        cart.User.FirstName ?? "Cliente",
                        cartSummary,
                        "https://tienda-ucn.cl/cart"
                    );
                    Log.Information("Sent abandoned cart reminder to: {Email}", cart.User.Email);
                }

                Log.Information("Job completed: reminders sent for {Count} abandoned carts", oldCarts.Count);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error while sending abandoned cart reminders.");
                throw;
            }
        }
    }
}
