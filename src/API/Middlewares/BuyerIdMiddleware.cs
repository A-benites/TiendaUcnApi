using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace TiendaUcnApi.src.API.Middleware
{
    public class BuyerIdMiddleware
    {
        private readonly RequestDelegate _next;

        public BuyerIdMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Busca la cookie "buyerId"
            var buyerId = context.Request.Cookies["buyerId"];

            // Si existe, la guarda en HttpContext.Items
            if (!string.IsNullOrEmpty(buyerId))
            {
                context.Items["BuyerId"] = buyerId;
            }

            // Continua el pipeline
            await _next(context);
        }
    }
}
