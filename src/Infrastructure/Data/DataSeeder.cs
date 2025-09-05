using Bogus;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TiendaUcnApi.src.Domain.Models;

namespace TiendaUcnApi.src.Infrastructure.Data;

public class DataSeeder
{
    public static async Task SeedAsync(AppDbContext context, UserManager<User> userManager, RoleManager<Role> roleManager)
    {
        // Aplica cualquier migración pendiente antes de poblar.
        await context.Database.MigrateAsync();

        // 1. Crear Roles según el taller ("Administrador", "Cliente")
        if (!await roleManager.Roles.AnyAsync())
        {
            await roleManager.CreateAsync(new Role { Name = "Administrador" });
            await roleManager.CreateAsync(new Role { Name = "Cliente" });
        }

        // 2. Crear Usuario Administrador según el taller
        if (!await userManager.Users.AnyAsync())
        {
            var adminUser = new User
            {
                UserName = "admin@tiendaucn.cl",
                Email = "admin@tiendaucn.cl",
                FirstName = "Admin",
                LastName = "Sistema",
                Rut = "1-9",
                Gender = Gender.Otro,
                BirthDate = new DateTime(1990, 1, 1),
                EmailConfirmed = true,
                IsSeed = true // Se asigna la propiedad IsSeed
            };

            await userManager.CreateAsync(adminUser, "Admin123!");
            await userManager.AddToRoleAsync(adminUser, "Administrador");
        }

        // 3. Crear Categorías, Marcas y Productos si la tabla de productos está vacía
        if (!await context.Products.AnyAsync())
        {
            // Generar 10 categorías con Bogus
            var categoryFaker = new Faker<Category>()
                .RuleFor(c => c.Name, f => f.Commerce.Department());
            var categories = categoryFaker.Generate(10);
            await context.Categories.AddRangeAsync(categories);

            // Generar 20 marcas con Bogus
            var brandFaker = new Faker<Brand>()
                .RuleFor(b => b.Name, f => f.Company.CompanyName());
            var brands = brandFaker.Generate(20);
            await context.Brands.AddRangeAsync(brands);

            await context.SaveChangesAsync(); // Guardar para obtener IDs

            // Generar 50 productos con Bogus
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