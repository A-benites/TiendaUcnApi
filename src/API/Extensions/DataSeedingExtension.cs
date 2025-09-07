// Extensions/DataSeedingExtension.cs
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TiendaUcnApi.src.Domain.Models;
using TiendaUcnApi.src.Infrastructure.Data;

namespace TiendaUcnApi.src.API.Extensions;

// This static class will hold our extension method.
public static class DataSeedingExtension
{
    // This is the extension method. It extends the IHost interface.
    public static async Task SeedDatabaseAsync(this IHost host)
    {
        // Create a scope to resolve services from the DI container.
        using (var scope = host.Services.CreateScope())
        {
            var services = scope.ServiceProvider;
            var logger = services.GetRequiredService<ILogger<Program>>();

            try
            {
                logger.LogInformation("Starting database seeding...");

                var context = services.GetRequiredService<AppDbContext>();
                var userManager = services.GetRequiredService<UserManager<User>>();
                var roleManager = services.GetRequiredService<RoleManager<Role>>();

                // Apply any pending migrations.
                await context.Database.MigrateAsync();

                // Call the seeder method to populate initial data.
                await DataSeeder.SeedAsync(context, userManager, roleManager);

                logger.LogInformation("Database seeding completed successfully.");
            }
            catch (Exception ex)
            {
                // Log any errors that occur during the seeding process.
                logger.LogError(ex, "An error occurred during database seeding.");
            }
        }
    }
}