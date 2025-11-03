using Bogus;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using TiendaUcnApi.src.Domain.Models;

namespace TiendaUcnApi.src.Infrastructure.Data;

/// <summary>
/// Class responsible for populating the database with initial data such as roles, admin user, categories, brands, and products.
/// Uses Bogus library to generate realistic fake data for development and testing.
/// </summary>
public class DataSeeder
{
    /// <summary>
    /// Populates the database with initial data, including roles, admin user, test customer, categories, brands, and products.
    /// This method is idempotent and only creates data if it doesn't already exist.
    /// </summary>
    /// <param name="context">Application database context.</param>
    /// <param name="userManager">User manager for creating users.</param>
    /// <param name="roleManager">Role manager for creating roles.</param>
    /// <param name="configuration">Application configuration containing admin user credentials.</param>
    public static async Task SeedAsync(
        AppDbContext context,
        UserManager<User> userManager,
        RoleManager<Role> roleManager,
        IConfiguration configuration
    )
    {
        // Apply any pending migrations before seeding.
        await context.Database.MigrateAsync();

        // 1. Create roles ("Administrador", "Cliente") if they don't exist
        if (!await roleManager.Roles.AnyAsync())
        {
            await roleManager.CreateAsync(new Role { Name = "Administrador" });
            await roleManager.CreateAsync(new Role { Name = "Cliente" });
        }

        // 2. Create admin user if no users exist
        if (!await userManager.Users.AnyAsync())
        {
            var adminUser = new User
            {
                UserName = configuration["User:AdminUser:Email"],
                Email = configuration["User:AdminUser:Email"],
                FirstName = configuration["User:AdminUser:FirstName"],
                LastName = configuration["User:AdminUser:LastName"],
                Rut = configuration["User:AdminUser:Rut"],
                Gender = Enum.Parse<Gender>(configuration["User:AdminUser:Gender"]),
                BirthDate = DateOnly.Parse(configuration["User:AdminUser:BirthDate"]),
                EmailConfirmed = true,
                IsSeed = true,
            };

            await userManager.CreateAsync(adminUser, configuration["User:AdminUser:Password"]);
            await userManager.AddToRoleAsync(adminUser, "Administrador");
        }

        // Create test customer user if it doesn't exist
        if (!await userManager.Users.AnyAsync(u => u.Email == "cliente@test.com"))
        {
            var customerUser = new User
            {
                UserName = "cliente@test.com",
                Email = "cliente@test.com",
                FirstName = "Cliente",
                LastName = "DePrueba",
                Rut = "55.555.555-5",
                Gender = Gender.Femenino,
                BirthDate = DateOnly.Parse("1998-05-10"),
                PhoneNumber = "+56955555555",
                EmailConfirmed = true,
                IsSeed = true,
            };

            await userManager.CreateAsync(customerUser, "Cliente123!");
            await userManager.AddToRoleAsync(customerUser, "Cliente");
        }

        // 3. Create categories, brands, and products if the products table is empty
        if (!await context.Products.AnyAsync())
        {
            // Generate 10 categories using Bogus
            var categoryFaker = new Faker<Category>().RuleFor(
                c => c.Name,
                f => f.Commerce.Department()
            );
            var categories = categoryFaker.Generate(10);
            await context.Categories.AddRangeAsync(categories);

            // Generate 20 brands using Bogus
            var brandFaker = new Faker<Brand>().RuleFor(b => b.Name, f => f.Company.CompanyName());
            var brands = brandFaker.Generate(20);
            await context.Brands.AddRangeAsync(brands);

            await context.SaveChangesAsync(); // Save to get the IDs

            // Generate 50 products using Bogus
            var productFaker = new Faker<Product>()
                .RuleFor(p => p.Title, f => f.Commerce.ProductName())
                .RuleFor(p => p.Description, f => f.Commerce.ProductDescription())
                .RuleFor(p => p.Price, f => decimal.Parse(f.Commerce.Price(1000, 200000)))
                .RuleFor(p => p.Stock, f => f.Random.Number(1, 100))
                .RuleFor(p => p.Status, f => f.PickRandom<Status>())
                .RuleFor(p => p.CategoryId, f => f.PickRandom(categories).Id)
                .RuleFor(p => p.BrandId, f => f.PickRandom(brands).Id);

            var products = productFaker.Generate(50);
            await context.Products.AddRangeAsync(products);

            await context.SaveChangesAsync();
        }
    }
}
