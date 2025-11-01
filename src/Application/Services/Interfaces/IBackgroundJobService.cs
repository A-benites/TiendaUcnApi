namespace TiendaUcnApi.src.Application.Services.Interfaces
{
    /// <summary>
    /// Interface that defines background jobs used by Hangfire.
    /// </summary>
    public interface IBackgroundJobService
    {
        /// <summary>
        /// Deletes unverified users after a given period.
        /// </summary>
        Task DeleteUnconfirmedUsersAsync();

        /// <summary>
        /// Sends email reminders for abandoned carts.
        /// </summary>
        Task SendAbandonedCartRemindersAsync();
    }
}
