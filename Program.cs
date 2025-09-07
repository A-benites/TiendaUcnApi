using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Serilog;
using TiendaUcnApi.src.API.Extensions; // Using for the new seeding extension method
using TiendaUcnApi.src.API.Middlewares.ErrorHandlingMiddleware;
using TiendaUcnApi.src.Domain.Models;
using TiendaUcnApi.src.Infrastructure.Data;

// Configure a bootstrap logger to capture errors during application startup.
Log.Logger = new LoggerConfiguration().WriteTo.Console().CreateBootstrapLogger();

Log.Information("Starting up the application");

try
{
    var builder = WebApplication.CreateBuilder(args);

    // Replace the default .NET logger with Serilog and read configuration from appsettings.json.
    builder.Host.UseSerilog(
        (context, services, configuration) =>
            configuration
                .ReadFrom.Configuration(context.Configuration)
                .ReadFrom.Services(services)
                .Enrich.FromLogContext()
    );

    // --- Service configuration for dependency injection (DI Container). ---

    // Registers the database context with the DI container.
    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"))
    );

    // Registers IHttpContextAccessor to allow access to the current HttpContext from services.
    builder.Services.AddHttpContextAccessor();

    // Configures ASP.NET Core Identity for user and role management.
    builder
        .Services.AddIdentity<User, Role>()
        .AddEntityFrameworkStores<AppDbContext>()
        .AddDefaultTokenProviders();

    // Configures controller services and customizes the automatic 400 response for validation errors.
    builder
        .Services.AddControllers()
        .ConfigureApiBehaviorOptions(options =>
        {
            options.InvalidModelStateResponseFactory = context =>
            {
                var errors = context
                    .ModelState.Where(e => e.Value.Errors.Count > 0)
                    .Select(e => new
                    {
                        Field = e.Key,
                        Errors = e.Value.Errors.Select(err => err.ErrorMessage),
                    });

                return new BadRequestObjectResult(
                    new
                    {
                        status = 400,
                        message = "Validation errors",
                        errors,
                        timestamp = DateTime.UtcNow,
                    }
                );
            };
        });

    // Adds services for API endpoint discovery and Swagger/OpenAPI documentation.
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    var app = builder.Build();

    // --- HTTP request pipeline configuration. Order is important. ---

    // Middleware to log all incoming HTTP requests automatically.
    app.UseSerilogRequestLogging();

    // Enables Swagger UI only in the development environment.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    // Custom middleware for global exception handling.
    app.UseMiddleware<ErrorHandlingMiddleware>();

    // Redirects HTTP requests to HTTPS.
    app.UseHttpsRedirection();

    // Enables authentication middleware.
    app.UseAuthentication();

    // Enables authorization middleware.
    app.UseAuthorization();

    // Maps controller action methods to endpoints.
    app.MapControllers();

    // Applies migrations and seeds the database on application startup.
    await app.SeedDatabaseAsync();

    // A test endpoint to verify the error handling middleware is working.
    app.MapGet(
        "/test",
        () =>
        {
            throw new Exception("Test exception for the middleware.");
        }
    );

    app.Run();
}
catch (Exception ex)
{
    // Logs fatal exceptions that prevent the application from starting.
    Log.Fatal(ex, "Application failed to start");
}
finally
{
    // Ensures all log events are written to the sinks before the application shuts down.
    Log.Information("Shutting down");
    Log.CloseAndFlush();
}