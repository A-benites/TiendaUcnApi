using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using TiendaUcnApi.src.Domain.Models;
using TiendaUcnApi.src.Infrastructure.Data;

namespace TiendaUcnApi.src.API.Extensions;

/// <summary>
/// Extension methods for database seeding during application startup.
/// </summary>
public static class DataSeedingExtension
{
    /// <summary>
    /// Seeds the database with initial data including roles, admin users, and sample products.
    /// This extension method applies pending migrations and populates the database with default data.
    /// </summary>
    /// <param name="host">The web application host.</param>
    /// <returns>A task representing the asynchronous seeding operation.</returns>
    public static async Task SeedDatabaseAsync(this IHost host)
    {
        // Create a scope to resolve services from the DI container
        using (var scope = host.Services.CreateScope())
        {
            var services = scope.ServiceProvider;
            var logger = services.GetRequiredService<ILogger<Program>>();
            var configuration = services.GetRequiredService<IConfiguration>();

            try
            {
                logger.LogInformation("Starting database seeding...");

                var context = services.GetRequiredService<AppDbContext>();
                var userManager = services.GetRequiredService<UserManager<User>>();
                var roleManager = services.GetRequiredService<RoleManager<Role>>();

                // Apply any pending migrations
                await context.Database.MigrateAsync();

                // Call the seeder method to populate initial data
                await DataSeeder.SeedAsync(context, userManager, roleManager, configuration);

                logger.LogInformation("Database seeding completed successfully.");
            }
            catch (Exception ex)
            {
                // Log any errors that occur during the seeding process
                logger.LogError(ex, "An error occurred during database seeding.");
            }
        }
    }
}
