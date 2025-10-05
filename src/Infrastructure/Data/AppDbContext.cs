using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TiendaUcnApi.src.Domain.Models;

namespace TiendaUcnApi.src.Infrastructure.Data
{
    /// <summary>
    /// Contexto de base de datos de la aplicación, integrando ASP.NET Core Identity.
    /// </summary>
    public class AppDbContext : IdentityDbContext<User, Role, int>
    {
        /// <summary>
        /// Inicializa una nueva instancia del contexto de base de datos.
        /// </summary>
        /// <param name="options">Opciones de configuración de DbContext.</param>
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }

        /// <summary>
        /// Tabla de productos.
        /// </summary>
        public DbSet<Product> Products { get; set; } = null!;

        /// <summary>
        /// Tabla de imágenes de productos.
        /// </summary>
        public DbSet<Image> Images { get; set; } = null!;

        /// <summary>
        /// Tabla de categorías de productos.
        /// </summary>
        public DbSet<Category> Categories { get; set; } = null!;

        /// <summary>
        /// Tabla de marcas de productos.
        /// </summary>
        public DbSet<Brand> Brands { get; set; } = null!;

        /// <summary>
        /// Tabla de órdenes de compra.
        /// </summary>
        public DbSet<Order> Orders { get; set; } = null!;

        /// <summary>
        /// Tabla de ítems de órdenes de compra.
        /// </summary>
        public DbSet<OrderItem> OrderItems { get; set; } = null!;

        /// <summary>
        /// Tabla de carritos de compra.
        /// </summary>
        public DbSet<Cart> Carts { get; set; } = null!;

        /// <summary>
        /// Tabla de ítems de carritos de compra.
        /// </summary>
        public DbSet<CartItem> CartItems { get; set; } = null!;

        /// <summary>
        /// Tabla de códigos de verificación.
        /// </summary>
        public DbSet<VerificationCode> VerificationCodes { get; set; } = null!;

        /// <summary>
        /// Configura el modelo de base de datos usando Fluent API para restricciones avanzadas.
        /// </summary>
        /// <param name="builder">Constructor de modelos de EF Core.</param>
        protected override void OnModelCreating(ModelBuilder builder)
        {
            // Es esencial llamar a la implementación base para que Identity configure su propio esquema.
            base.OnModelCreating(builder);

            // Configuraciones para la entidad 'User'
            builder.Entity<User>(entity =>
            {
                // Define índices únicos para asegurar que no haya emails o RUTs duplicados a nivel de base de datos.
                entity.HasIndex(u => u.Email).IsUnique();
                entity.HasIndex(u => u.Rut).IsUnique();
            });
        }
    }
}