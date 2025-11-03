using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace TiendaUcnApi.src.API.Middleware
{
    /// <summary>
    /// Middleware that extracts and stores the anonymous buyer identifier from cookies.
    /// Used to track shopping carts for users who haven't logged in yet.
    /// </summary>
    public class BuyerIdMiddleware
    {
        private readonly RequestDelegate _next;

        /// <summary>
        /// Initializes a new instance of the <see cref="BuyerIdMiddleware"/> class.
        /// </summary>
        /// <param name="next">The next middleware delegate in the pipeline.</param>
        public BuyerIdMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        /// <summary>
        /// Processes an HTTP request by extracting the buyerId cookie and storing it in HttpContext.Items.
        /// </summary>
        /// <param name="context">The HTTP context for the current request.</param>
        public async Task InvokeAsync(HttpContext context)
        {
            // Look for the "buyerId" cookie
            var buyerId = context.Request.Cookies["buyerId"];

            // If it exists, store it in HttpContext.Items for access in controllers/services
            if (!string.IsNullOrEmpty(buyerId))
            {
                context.Items["BuyerId"] = buyerId;
            }

            // Continue the pipeline
            await _next(context);
        }
    }
}
