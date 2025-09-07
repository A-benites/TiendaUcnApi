using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TiendaUcnApi.src.Domain.Models;
using TiendaUcnApi.src.Infrastructure.Data;
using TiendaUcnApi.src.API.Middlewares.ErrorHandlingMiddleware;
using Microsoft.AspNetCore.Mvc;
var builder = WebApplication.CreateBuilder(args);

// Service configuration for dependency injection.
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"))
);

builder
    .Services.AddIdentity<User, Role>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddControllers()
    .ConfigureApiBehaviorOptions(options =>
    {
        options.InvalidModelStateResponseFactory = context =>
        {
            var errors = context.ModelState
                .Where(e => e.Value.Errors.Count > 0)
                .Select(e => new
                {
                    Field = e.Key,
                    Errors = e.Value.Errors.Select(err => err.ErrorMessage)
                });

            return new BadRequestObjectResult(new
            {
                status = 400,
                message = "Validation errors",
                errors,
                timestamp = DateTime.UtcNow
            });
        };
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// HTTP request pipeline configuration. Order is important.
if (app.Environment.IsDevelopment())
{
    // Enables middleware to generate OpenAPI specification and Swagger UI.
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ErrorHandlingMiddleware>();

// Redirects HTTP requests to HTTPS.
app.UseHttpsRedirection();

// Enables authentication.
app.UseAuthentication();

// Enables authorization. Must be declared after UseAuthentication.
app.UseAuthorization();

// Maps endpoints to controller action methods.
app.MapControllers();

// Applies migrations and seeds the database on startup.
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<AppDbContext>();
        var userManager = services.GetRequiredService<UserManager<User>>();
        var roleManager = services.GetRequiredService<RoleManager<Role>>();

        // Calls the seeder method.
        await DataSeeder.SeedAsync(context, userManager, roleManager);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred during database seeding.");
    }
}

app.MapGet("/test", () =>
{
    // Aquí puedes lanzar un error a propósito
    throw new Exception("Error de prueba del middleware");
});

app.Run();