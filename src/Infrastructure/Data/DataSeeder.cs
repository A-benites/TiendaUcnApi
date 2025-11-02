using Bogus;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using TiendaUcnApi.src.Domain.Models;

namespace TiendaUcnApi.src.Infrastructure.Data;

/// <summary>
/// Clase encargada de poblar la base de datos con datos iniciales como roles, usuario administrador, categorías, marcas y productos.
/// </summary>
public class DataSeeder
{
    /// <summary>
    /// Pobla la base de datos con datos iniciales, incluyendo roles, usuario administrador, categorías, marcas y productos.
    /// </summary>
    /// <param name="context">Contexto de la base de datos de la aplicación.</param>
    /// <param name="userManager">Gestor de usuarios para crear usuarios.</param>
    /// <param name="roleManager">Gestor de roles para crear roles.</param>
    /// <param name="configuration">Configuración de la aplicación.</param>
    public static async Task SeedAsync(
        AppDbContext context,
        UserManager<User> userManager,
        RoleManager<Role> roleManager,
        IConfiguration configuration
    )
    {
        // Aplica migraciones pendientes antes de poblar.
        await context.Database.MigrateAsync();

        // 1. Crea roles ("Administrador", "Cliente") si no existen
        if (!await roleManager.Roles.AnyAsync())
        {
            await roleManager.CreateAsync(new Role { Name = "Administrador" });
            await roleManager.CreateAsync(new Role { Name = "Cliente" });
        }

        // 2. Crea usuario administrador si no existen usuarios
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

        // Crea usuario cliente de prueba si no existe
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

        // 3. Crea categorías, marcas y productos si la tabla de productos está vacía
        if (!await context.Products.AnyAsync())
        {
            // Genera 10 categorías usando Bogus
            var categoryFaker = new Faker<Category>().RuleFor(
                c => c.Name,
                f => f.Commerce.Department()
            );
            var categories = categoryFaker.Generate(10);
            await context.Categories.AddRangeAsync(categories);

            // Genera 20 marcas usando Bogus
            var brandFaker = new Faker<Brand>().RuleFor(b => b.Name, f => f.Company.CompanyName());
            var brands = brandFaker.Generate(20);
            await context.Brands.AddRangeAsync(brands);

            await context.SaveChangesAsync(); // Guarda para obtener los IDs

            // Genera 50 productos usando Bogus
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
