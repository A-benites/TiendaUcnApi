using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TiendaUcnApi.src.Domain.Models;

namespace TiendaUcnApi.src.Infrastructure.Data
{
    /// <summary>
    /// Application database context integrating ASP.NET Core Identity.
    /// </summary>
    public class AppDbContext : IdentityDbContext<User, Role, int>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }

        // DbSet for each entity that will be a table in the database.
        // "= null!;" suppresses a compiler nullability warning. EF Core will initialize them.
        public DbSet<Product> Products { get; set; } = null!;
        public DbSet<Image> Images { get; set; } = null!;
        public DbSet<Category> Categories { get; set; } = null!;
        public DbSet<Brand> Brands { get; set; } = null!;
        public DbSet<Order> Orders { get; set; } = null!;
        public DbSet<OrderItem> OrderItems { get; set; } = null!;
        public DbSet<Cart> Carts { get; set; } = null!;
        public DbSet<CartItem> CartItems { get; set; } = null!;
        public DbSet<VerificationCode> VerificationCodes { get; set; } = null!;

        /// <summary>
        /// Configures the database model using Fluent API for advanced constraints.
        /// </summary>
        protected override void OnModelCreating(ModelBuilder builder)
        {
            // It is essential to call the base implementation so Identity configures its own schema.
            base.OnModelCreating(builder);

            // Configurations for the 'User' entity
            builder.Entity<User>(entity =>
            {
                // Define unique indexes to ensure no duplicate emails or RUTs at the database level.
                entity.HasIndex(u => u.Email).IsUnique();
                entity.HasIndex(u => u.Rut).IsUnique();
            });
        }
    }
}