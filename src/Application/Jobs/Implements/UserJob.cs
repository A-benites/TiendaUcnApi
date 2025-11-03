using Hangfire;
using Serilog;
using TiendaUcnApi.src.Application.Jobs.Interfaces;
using TiendaUcnApi.src.Application.Services.Interfaces;

namespace TiendaUcnApi.src.Application.Jobs
{
    /// <summary>
    /// Implementation of user background jobs executed by Hangfire.
    /// Handles scheduled tasks related to user account management.
    /// </summary>
    public class UserJob : IUserJob
    {
        private readonly IUserService _userService;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserJob"/> class.
        /// </summary>
        /// <param name="userService">The user service for performing user operations.</param>
        /// <param name="_configuration">Application configuration (currently unused).</param>
        public UserJob(IUserService userService, IConfiguration _configuration)
        {
            _userService = userService;
        }

        /// <summary>
        /// Deletes unconfirmed user accounts that have not verified their email.
        /// Automatically retries on failure with exponential backoff.
        /// </summary>
        [AutomaticRetry(Attempts = 10, DelaysInSeconds = new int[] { 60, 120, 300, 600, 900 })]
        public async Task DeleteUnconfirmedAsync()
        {
            Log.Information("Deleting unconfirmed users...");
            await _userService.DeleteUnconfirmedAsync();
        }
    }
}
