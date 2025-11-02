namespace TiendaUcnApi.src.Application.Jobs.Interfaces
{
    /// <summary>
    /// Interface for user-related background jobs executed by Hangfire.
    /// </summary>
    public interface IUserJob
    {
        /// <summary>
        /// Deletes unconfirmed user accounts that have not verified their email within the allowed timeframe.
        /// </summary>
        Task DeleteUnconfirmedAsync();
    }
}
