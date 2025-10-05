using Microsoft.EntityFrameworkCore;
using TiendaUcnApi.src.Application.DTO.ProductDTO;
using TiendaUcnApi.src.Domain.Models;
using TiendaUcnApi.src.Infrastructure.Data;
using TiendaUcnApi.src.Infrastructure.Repositories.Interfaces;

namespace TiendaUcnApi.src.Infrastructure.Repositories.Implements;

/// <summary>
/// Implementación del repositorio de productos que interactúa con la base de datos.
/// </summary>
public class ProductRepository : IProductRepository
{
    /// <summary>
    /// Contexto de base de datos de la aplicación.
    /// </summary>
    private readonly AppDbContext _context;

    /// <summary>
    /// Configuración de la aplicación.
    /// </summary>
    private readonly IConfiguration _configuration;

    /// <summary>
    /// Tamaño de página por defecto.
    /// </summary>
    private readonly int _defaultPageSize;

    /// <summary>
    /// Constructor que inyecta dependencias necesarias.
    /// </summary>
    /// <param name="context">Contexto de base de datos.</param>
    /// <param name="configuration">Configuración de la aplicación.</param>
    public ProductRepository(AppDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
        _defaultPageSize =
            _configuration.GetValue<int?>("Products:DefaultPageSize")
            ?? throw new ArgumentNullException(
                "El tamaño de página por defecto no puede ser nulo."
            );
    }

    /// <summary>
    /// Crea un nuevo producto en el repositorio.
    /// </summary>
    /// <param name="product">El producto a crear.</param>
    /// <returns>Id del producto creado.</returns>
    public async Task<int> CreateAsync(Product product)
    {
        await _context.Products.AddAsync(product);
        await _context.SaveChangesAsync();
        return product.Id;
    }

    /// <summary>
    /// Obtiene una marca por su Id.
    /// </summary>
    /// <param name="id">Id de la marca.</param>
    /// <returns>Marca encontrada.</returns>
    public async Task<Brand?> GetBrandByIdAsync(int id)
    {
        return await _context.Brands.AsNoTracking().FirstOrDefaultAsync(b => b.Id == id);
    }

    /// <summary>
    /// Obtiene una categoría por su Id.
    /// </summary>
    /// <param name="id">Id de la categoría.</param>
    /// <returns>Categoría encontrada.</returns>
    public async Task<Category?> GetCategoryByIdAsync(int id)
    {
        return await _context.Categories.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id);
    }

    /// <summary>
    /// Retorna un producto específico por su ID.
    /// </summary>
    /// <param name="id">ID del producto a buscar.</param>
    /// <returns>Producto encontrado o null si no existe.</returns>
    public async Task<Product?> GetByIdAsync(int id)
    {
        return await _context
            .Products.AsNoTracking()
            .Where(p => p.Id == id && p.IsAvailable)
            .Include(p => p.Category)
            .Include(p => p.Brand)
            .Include(p => p.Images)
            .FirstOrDefaultAsync();
    }

    /// <summary>
    /// Retorna un producto específico por su ID desde el punto de vista de un admin.
    /// </summary>
    /// <param name="id">ID del producto a buscar.</param>
    /// <returns>Producto encontrado o null si no existe.</returns>
    public async Task<Product?> GetByIdForAdminAsync(int id)
    {
        return await _context
            .Products.AsNoTracking()
            .Where(p => p.Id == id)
            .Include(p => p.Category)
            .Include(p => p.Brand)
            .Include(p => p.Images)
            .FirstOrDefaultAsync();
    }

    /// <summary>
    /// Retorna una lista de productos para el administrador con los parámetros de búsqueda especificados.
    /// </summary>
    /// <param name="searchParams">Parámetros de búsqueda para filtrar los productos.</param>
    /// <returns>Lista de productos y el conteo total.</returns>
    public async Task<(IEnumerable<Product> products, int totalCount)> GetFilteredForAdminAsync(
        SearchParamsDTO searchParams
    )
    {
        var query = _context
            .Products.Include(p => p.Category)
            .Include(p => p.Brand)
            .Include(p => p.Images.OrderBy(i => i.CreatedAt).Take(1))
            .AsNoTracking();

        if (!string.IsNullOrWhiteSpace(searchParams.SearchTerm))
        {
            var searchTerm = searchParams.SearchTerm.Trim().ToLower();

            query = query.Where(p =>
                p.Title.ToLower().Contains(searchTerm)
                || p.Description.ToLower().Contains(searchTerm)
                || p.Category.Name.ToLower().Contains(searchTerm)
                || p.Brand.Name.ToLower().Contains(searchTerm)
                || p.Status.ToString().ToLower().Contains(searchTerm)
                || p.Price.ToString().ToLower().Contains(searchTerm)
                || p.Stock.ToString().ToLower().Contains(searchTerm)
            );
        }

        var products = await query
            .OrderByDescending(p => p.CreatedAt)
            .Skip((searchParams.PageNumber - 1) * searchParams.PageSize ?? _defaultPageSize)
            .Take(searchParams.PageSize ?? _defaultPageSize)
            .ToArrayAsync();
        int totalCount = await query.CountAsync();
        return (products, totalCount);
    }

    /// <summary>
    /// Retorna una lista de productos para el cliente con los parámetros de búsqueda especificados.
    /// </summary>
    /// <param name="searchParams">Parámetros de búsqueda para filtrar los productos.</param>
    /// <returns>Lista de productos y el conteo total.</returns>
    public async Task<(IEnumerable<Product> products, int totalCount)> GetFilteredForCustomerAsync(
        SearchParamsDTO searchParams
    )
    {
        var query = _context
            .Products.Where(p => p.IsAvailable)
            .Include(p => p.Category)
            .Include(p => p.Brand)
            .Include(p => p.Images.OrderBy(i => i.CreatedAt).Take(1))
            .AsNoTracking();

        if (!string.IsNullOrWhiteSpace(searchParams.SearchTerm))
        {
            var searchTerm = searchParams.SearchTerm.Trim().ToLower();

            query = query.Where(p =>
                p.Title.ToLower().Contains(searchTerm)
                || p.Description.ToLower().Contains(searchTerm)
                || p.Category.Name.ToLower().Contains(searchTerm)
                || p.Brand.Name.ToLower().Contains(searchTerm)
                || p.Status.ToString().ToLower().Contains(searchTerm)
                || p.Price.ToString().ToLower().Contains(searchTerm)
                || p.Stock.ToString().ToLower().Contains(searchTerm)
            );
        }
        int totalCount = await query.CountAsync();
        var products = await query
            .OrderByDescending(p => p.CreatedAt)
            .Skip((searchParams.PageNumber - 1) * searchParams.PageSize ?? _defaultPageSize)
            .Take(searchParams.PageSize ?? _defaultPageSize)
            .ToArrayAsync();

        return (products, totalCount);
    }

    /// <summary>
    /// Obtiene el stock real de un producto por su ID.
    /// </summary>
    /// <param name="productId">ID del producto.</param>
    /// <returns>Stock real del producto.</returns>
    public async Task<int> GetRealStockAsync(int productId)
    {
        return await _context
            .Products.AsNoTracking()
            .Where(p => p.Id == productId)
            .Select(p => p.Stock)
            .FirstOrDefaultAsync();
    }

    /// <summary>
    /// Cambia el estado activo de un producto por su ID.
    /// </summary>
    /// <param name="id">ID del producto.</param>
    public async Task ToggleActiveAsync(int id)
    {
        await _context
            .Products.Where(p => p.Id == id)
            .ExecuteUpdateAsync(p => p.SetProperty(p => p.IsAvailable, p => !p.IsAvailable));
    }

    /// <summary>
    /// Actualiza el stock de un producto por su ID.
    /// </summary>
    /// <param name="productId">ID del producto.</param>
    /// <param name="stock">Nuevo stock.</param>
    public async Task UpdateStockAsync(int productId, int stock)
    {
        Product? product =
            await _context.Products.FindAsync(productId)
            ?? throw new KeyNotFoundException("Producto no encontrado");
        product.Stock = stock;
        await _context.SaveChangesAsync();
    }

    /// <summary>
    /// Actualiza los datos de un producto.
    /// </summary>
    /// <param name="product">Producto con los datos actualizados.</param>
    /// <returns>Producto actualizado.</returns>
    public async Task<Product> UpdateAsync(Product product)
    {
        product.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return product;
    }

    /// <summary>
    /// Actualiza el descuento de un producto.
    /// </summary>
    /// <param name="productId">ID del producto.</param>
    /// <param name="discount">Nuevo descuento.</param>
    public async Task UpdateDiscountAsync(int productId, int discount)
    {
        await _context
            .Products.Where(p => p.Id == productId)
            .ExecuteUpdateAsync(s => s.SetProperty(p => p.Discount, discount));
    }

    /// <summary>
    /// Obtiene un producto con seguimiento de cambios por su ID para administración.
    /// </summary>
    /// <param name="id">ID del producto.</param>
    /// <returns>Producto encontrado o null si no existe.</returns>
    public async Task<Product?> GetTrackedByIdForAdminAsync(int id)
    {
        return await _context
            .Products.Where(p => p.Id == id)
            .Include(p => p.Category)
            .Include(p => p.Brand)
            .Include(p => p.Images)
            .FirstOrDefaultAsync();
    }
}