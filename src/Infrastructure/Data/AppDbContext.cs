using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TiendaUcnApi.src.Domain.Models;

namespace TiendaUcnApi.src.Infrastructure.Data
{
    /// <summary>
    /// Contexto de la base de datos de la aplicación que integra ASP.NET Core Identity.
    /// </summary>
    public class AppDbContext : IdentityDbContext<User, Role, int>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }

        // DbSet para cada entidad que será una tabla en la base de datos.
        // El "= null!;" suprime una advertencia de nulabilidad del compilador. EF Core se encarga de inicializarlos.
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
        /// Configura el modelo de la base de datos usando Fluent API para definir restricciones avanzadas.
        /// </summary>
        protected override void OnModelCreating(ModelBuilder builder)
        {
            // Es indispensable llamar a la implementación base para que Identity configure su propio esquema.
            base.OnModelCreating(builder);

            // Configuraciones para la entidad 'User'
            builder.Entity<User>(entity =>
            {
                // Define índices únicos para garantizar que no haya emails o RUTs duplicados a nivel de base de datos.
                entity.HasIndex(u => u.Email).IsUnique();
                entity.HasIndex(u => u.Rut).IsUnique();
            });
        }
    }
}