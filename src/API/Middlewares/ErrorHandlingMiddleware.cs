using System.Security;
using System.Text.Json;
using Serilog;
using TiendaUcnApi.src.Application.DTO.BaseResponse;
using TiendaUcnApi.src.Application.Exceptions;

namespace TiendaUcnApi.src.API.Middlewares.ErrorHandlingMiddleware;

/// <summary>
/// Global exception handling middleware that catches unhandled exceptions and converts them to standardized HTTP responses.
/// Provides consistent error formatting and logging across the application.
/// </summary>
public class ErrorHandlingMiddleware(RequestDelegate next)
{
    private readonly RequestDelegate _next = next;

    /// <summary>
    /// Processes an HTTP request, handling any exceptions that occur during execution.
    /// </summary>
    /// <param name="context">The HTTP context for the current request.</param>
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);

            // Handle 401 Unauthorized responses with custom message
            if (context.Response.StatusCode == StatusCodes.Status401Unauthorized)
            {
                context.Response.ContentType = "application/json";
                var result = System.Text.Json.JsonSerializer.Serialize(
                    new
                    {
                        message = "You are not authorized to perform this action. Please log in.",
                    }
                );
                await context.Response.WriteAsync(result);
            }
        }
        catch (Exception ex)
        {
            // Capture unhandled exceptions and generate a unique trace ID for tracking
            var traceId = Guid.NewGuid().ToString();
            context.Response.Headers["trace-id"] = traceId;

            var (statusCode, title) = MapExceptionToStatus(ex);

            // Create an ErrorDetail object for the response
            ErrorDetail error = new ErrorDetail(title, ex.Message);

            Log.Error(ex, "Unhandled exception. Trace ID: {TraceId}", traceId);

            // Configure HTTP response as JSON
            context.Response.ContentType = "application/json";
            // Set the appropriate HTTP status code
            context.Response.StatusCode = statusCode;

            // Serialize the ErrorDetail object to JSON and write it to the response
            var json = JsonSerializer.Serialize(
                error,
                new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }
            );

            // Write the response to the client
            await context.Response.WriteAsync(json);
        }
    }

    /// <summary>
    /// Maps exception types to HTTP status codes and user-friendly titles.
    /// </summary>
    /// <param name="ex">The exception to map.</param>
    /// <returns>A tuple containing the HTTP status code and error title.</returns>
    private static (int, string) MapExceptionToStatus(Exception ex)
    {
        return ex switch
        {
            ConflictException _ => (StatusCodes.Status409Conflict, "Conflict"),
            UnauthorizedAccessException _ => (StatusCodes.Status401Unauthorized, "Unauthorized"),
            ArgumentNullException _ => (StatusCodes.Status400BadRequest, "Invalid Request"),
            KeyNotFoundException _ => (StatusCodes.Status404NotFound, "Resource Not Found"),
            InvalidOperationException _ => (StatusCodes.Status409Conflict, "Operation Conflict"),
            FormatException _ => (StatusCodes.Status400BadRequest, "Invalid Format"),
            SecurityException _ => (StatusCodes.Status403Forbidden, "Access Forbidden"),
            ArgumentOutOfRangeException _ => (
                StatusCodes.Status400BadRequest,
                "Argument Out of Range"
            ),
            ArgumentException _ => (StatusCodes.Status400BadRequest, "Invalid Argument"),
            TimeoutException _ => (StatusCodes.Status429TooManyRequests, "Too Many Requests"),
            JsonException _ => (StatusCodes.Status400BadRequest, "Invalid JSON"),
            _ => (StatusCodes.Status500InternalServerError, "Internal Server Error"),
        };
    }
}
