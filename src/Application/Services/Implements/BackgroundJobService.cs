using Serilog;
using TiendaUcnApi.src.Application.Services.Interfaces;
using TiendaUcnApi.src.Domain.Models;
using TiendaUcnApi.src.Infrastructure.Repositories.Interfaces;

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

        /// <summary>
        /// Initializes a new instance of the BackgroundJobService class.
        /// </summary>
        /// <param name="userRepository">User repository.</param>
        /// <param name="cartRepository">Cart repository.</param>
        /// <param name="emailService">Email service.</param>
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
        /// Scheduled to run daily via Hangfire.
        /// </summary>
        public async Task DeleteUnconfirmedUsersAsync()
        {
            try
            {
                var deletedCount = await _userRepository.DeleteUnconfirmedAsync();
                Log.Information("Job completed: deleted {Count} unverified users", deletedCount);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error while deleting unverified users.");
                throw;
            }
        }

        /// <summary>
        /// Sends reminder emails for abandoned carts (not updated in the last 3 days).
        /// Scheduled to run daily via Hangfire to re-engage customers.
        /// </summary>
        public async Task SendAbandonedCartRemindersAsync()
        {
            try
            {
                // Retrieve abandoned carts directly from repository
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

                    // Generate HTML list of products
                    var cartItemsList = new List<string>();
                    foreach (var item in cart.CartItems)
                    {
                        var title = item.Product?.Title ?? "Producto desconocido";
                        cartItemsList.Add($"• {title} - {item.Quantity} unidades");
                    }

                    var cartSummary = string.Join("<br>", cartItemsList);

                    // Send reminder email
                    await _emailService.SendAbandonedCartReminderAsync(
                        cart.User.Email,
                        cart.User.FirstName ?? "Cliente",
                        cartSummary,
                        "https://tienda-ucn.cl/cart"
                    );

                    Log.Information("Sent abandoned cart reminder to: {Email}", cart.User.Email);
                }

                Log.Information(
                    "Job completed: reminders sent for {Count} abandoned carts",
                    abandonedCarts.Count
                );
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error while sending abandoned cart reminders.");
                throw;
            }
        }
    }
}
