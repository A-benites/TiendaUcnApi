using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TiendaUcnApi.src.Domain.Models;

namespace TiendaUcnApi.src.Infrastructure.Data
{
    /// <summary>
    /// Application database context, integrating ASP.NET Core Identity.
    /// Manages all database entities and their relationships using Entity Framework Core.
    /// </summary>
    public class AppDbContext : IdentityDbContext<User, Role, int>
    {
        /// <summary>
        /// Initializes a new instance of the AppDbContext class.
        /// </summary>
        /// <param name="options">DbContext configuration options.</param>
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }

        /// <summary>
        /// Products table.
        /// </summary>
        public DbSet<Product> Products { get; set; } = null!;

        /// <summary>
        /// Audit records table for tracking administrative actions.
        /// </summary>
        public DbSet<AuditRecord> AuditRecords { get; set; } = null!;

        /// <summary>
        /// Product images table.
        /// </summary>
        public DbSet<Image> Images { get; set; } = null!;

        /// <summary>
        /// Product categories table.
        /// </summary>
        public DbSet<Category> Categories { get; set; } = null!;

        /// <summary>
        /// Product brands table.
        /// </summary>
        public DbSet<Brand> Brands { get; set; } = null!;

        /// <summary>
        /// Purchase orders table.
        /// </summary>
        public DbSet<Order> Orders { get; set; } = null!;

        /// <summary>
        /// Order items table.
        /// </summary>
        public DbSet<OrderItem> OrderItems { get; set; } = null!;

        /// <summary>
        /// Shopping carts table.
        /// </summary>
        public DbSet<Cart> Carts { get; set; } = null!;

        /// <summary>
        /// Cart items table.
        /// </summary>
        public DbSet<CartItem> CartItems { get; set; } = null!;

        /// <summary>
        /// Verification codes table.
        /// </summary>
        public DbSet<VerificationCode> VerificationCodes { get; set; } = null!;

        /// <summary>
        /// Configures the database model using Fluent API for advanced constraints.
        /// </summary>
        /// <param name="builder">EF Core model builder.</param>
        protected override void OnModelCreating(ModelBuilder builder)
        {
            // Essential to call the base implementation so that Identity configures its own schema.
            base.OnModelCreating(builder);

            // Configuration for the 'User' entity
            builder.Entity<User>(entity =>
            {
                // Define unique indexes to ensure no duplicate emails or RUTs at the database level.
                entity.HasIndex(u => u.Email).IsUnique();
                entity.HasIndex(u => u.Rut).IsUnique();
            });
        }
    }
}
