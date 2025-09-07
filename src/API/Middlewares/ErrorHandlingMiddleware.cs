using System.Security;
using System.Text.Json;
using Serilog;

namespace TiendaUcnApi.src.API.Middlewares.ErrorHandlingMiddleware;
public class ErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ErrorHandlingMiddleware> _logger;

    public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception");

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = GetStatusCode(ex);

            var errorResponse = new
            {
                status = context.Response.StatusCode,
                message = ex.Message,
                errorId = Guid.NewGuid().ToString(),
                timestamp = DateTime.UtcNow
            };

            await context.Response.WriteAsJsonAsync(errorResponse);
        }
    }

    private int GetStatusCode(Exception ex)
    {
        return ex switch
        {
            ArgumentException => StatusCodes.Status400BadRequest,
            UnauthorizedAccessException => StatusCodes.Status401Unauthorized,
            SecurityException _ => StatusCodes.Status403Forbidden,
            KeyNotFoundException => StatusCodes.Status404NotFound,
            InvalidOperationException => StatusCodes.Status409Conflict,
            _ => StatusCodes.Status500InternalServerError
        };
    }
}