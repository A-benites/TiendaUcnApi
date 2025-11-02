using Microsoft.EntityFrameworkCore;
using TiendaUcnApi.src.Application.DTO.ProductDTO;
using TiendaUcnApi.src.Domain.Models;
using TiendaUcnApi.src.Infrastructure.Data;
using TiendaUcnApi.src.Infrastructure.Repositories.Interfaces;

namespace TiendaUcnApi.src.Infrastructure.Repositories.Implements;

/// <summary>
/// Implementation of the product repository that interacts with the database.
/// Handles product CRUD operations, filtering, search, and availability management.
/// </summary>
public class ProductRepository : IProductRepository
{
    /// <summary>
    /// Application database context.
    /// </summary>
    private readonly AppDbContext _context;

    /// <summary>
    /// Application configuration.
    /// </summary>
    private readonly IConfiguration _configuration;

    /// <summary>
    /// Default page size for pagination.
    /// </summary>
    private readonly int _defaultPageSize;

    /// <summary>
    /// Initializes a new instance of the <see cref="ProductRepository"/> class.
    /// </summary>
    /// <param name="context">Database context.</param>
    /// <param name="configuration">Application configuration.</param>
    public ProductRepository(AppDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
        _defaultPageSize =
            _configuration.GetValue<int?>("Products:DefaultPageSize")
            ?? throw new ArgumentNullException("The default page size cannot be null.");
    }

    /// <summary>
    /// Creates a new product in the repository.
    /// </summary>
    /// <param name="product">The product to create.</param>
    /// <returns>ID of the created product.</returns>
    public async Task<int> CreateAsync(Product product)
    {
        await _context.Products.AddAsync(product);
        await _context.SaveChangesAsync();
        return product.Id;
    }

    /// <summary>
    /// Retrieves a brand by its ID.
    /// </summary>
    /// <param name="id">Brand ID.</param>
    /// <returns>The found brand or null.</returns>
    public async Task<Brand?> GetBrandByIdAsync(int id)
    {
        return await _context.Brands.AsNoTracking().FirstOrDefaultAsync(b => b.Id == id);
    }

    /// <summary>
    /// Retrieves a category by its ID.
    /// </summary>
    /// <param name="id">Category ID.</param>
    /// <returns>The found category or null.</returns>
    public async Task<Category?> GetCategoryByIdAsync(int id)
    {
        return await _context.Categories.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id);
    }

    /// <summary>
    /// Retrieves a specific product by its ID.
    /// </summary>
    /// <param name="id">Product ID to search for.</param>
    /// <returns>The found product or null if it doesn't exist.</returns>
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
    /// Retrieves a specific product by ID from an administrator's perspective.
    /// Includes all product details regardless of availability status.
    /// </summary>
    /// <param name="id">Product ID to search for.</param>
    /// <returns>Product found or null if it doesn't exist.</returns>
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
    /// Retrieves a specific product by ID from a customer's perspective.
    /// Only returns products that are available for purchase (IsAvailable = true).
    /// </summary>
    /// <param name="id">Product ID to search for.</param>
    /// <returns>Product found or null if it doesn't exist or is not available.</returns>
    public async Task<Product?> GetByIdForCustomerAsync(int id)
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
    /// Retrieves a filtered list of products for administrators with specified search parameters.
    /// Includes all products regardless of availability status with full search capabilities.
    /// </summary>
    /// <param name="searchParams">Search parameters for filtering products (term, pagination).</param>
    /// <returns>Tuple containing the list of products and the total count.</returns>
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
    /// Retrieves a filtered list of products for customers with specified search parameters.
    /// Only includes available products. Supports filtering by category, brand, price range, status, and sorting.
    /// Implements requirements R68 (filtering) and R70 (sorting).
    /// </summary>
    /// <param name="searchParams">Search parameters including filters, sorting, and pagination.</param>
    /// <returns>Tuple containing the list of available products and the total count.</returns>
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

        // Search term filter
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

        // R68: Category filter
        if (searchParams.CategoryId.HasValue)
        {
            query = query.Where(p => p.CategoryId == searchParams.CategoryId.Value);
        }

        // R68: Brand filter
        if (searchParams.BrandId.HasValue)
        {
            query = query.Where(p => p.BrandId == searchParams.BrandId.Value);
        }

        // R68: Price range filter
        if (searchParams.MinPrice.HasValue)
        {
            query = query.Where(p => p.Price >= searchParams.MinPrice.Value);
        }

        if (searchParams.MaxPrice.HasValue)
        {
            query = query.Where(p => p.Price <= searchParams.MaxPrice.Value);
        }

        // R68: Status filter
        if (searchParams.Status.HasValue)
        {
            query = query.Where(p => p.Status == searchParams.Status.Value);
        }

        // Get total count before pagination
        int totalCount = await query.CountAsync();

        // R70: Apply sorting based on SortBy parameter
        query = searchParams.SortBy switch
        {
            ProductSortOption.PriceAsc => query.OrderBy(p => p.Price),
            ProductSortOption.PriceDesc => query.OrderByDescending(p => p.Price),
            ProductSortOption.NameAsc => query.OrderBy(p => p.Title),
            ProductSortOption.NameDesc => query.OrderByDescending(p => p.Title),
            ProductSortOption.Newest or _ => query.OrderByDescending(p => p.CreatedAt),
        };

        // Apply pagination
        var products = await query
            .Skip((searchParams.PageNumber - 1) * searchParams.PageSize ?? _defaultPageSize)
            .Take(searchParams.PageSize ?? _defaultPageSize)
            .ToArrayAsync();

        return (products, totalCount);
    }

    /// <summary>
    /// Retrieves the real stock of a product by its ID.
    /// Used for stock verification during cart and order operations.
    /// </summary>
    /// <param name="productId">Product ID.</param>
    /// <returns>Product's current stock quantity.</returns>
    public async Task<int> GetRealStockAsync(int productId)
    {
        return await _context
            .Products.AsNoTracking()
            .Where(p => p.Id == productId)
            .Select(p => p.Stock)
            .FirstOrDefaultAsync();
    }

    /// <summary>
    /// Toggles the active status of a product by its ID.
    /// Switches IsAvailable between true and false.
    /// </summary>
    /// <param name="id">Product ID.</param>
    public async Task ToggleActiveAsync(int id)
    {
        await _context
            .Products.Where(p => p.Id == id)
            .ExecuteUpdateAsync(p => p.SetProperty(p => p.IsAvailable, p => !p.IsAvailable));
    }

    /// <summary>
    /// Updates the stock of a product by its ID.
    /// Throws KeyNotFoundException if the product is not found.
    /// </summary>
    /// <param name="productId">Product ID.</param>
    /// <param name="stock">New stock quantity.</param>
    /// <exception cref="KeyNotFoundException">Thrown when the product is not found.</exception>
    public async Task UpdateStockAsync(int productId, int stock)
    {
        Product? product =
            await _context.Products.FindAsync(productId)
            ?? throw new KeyNotFoundException("Product not found");
        product.Stock = stock;
        await _context.SaveChangesAsync();
    }

    /// <summary>
    /// Updates product data.
    /// Automatically sets UpdatedAt to the current UTC time.
    /// </summary>
    /// <param name="product">Product entity with updated data.</param>
    /// <returns>Updated product.</returns>
    public async Task<Product> UpdateAsync(Product product)
    {
        product.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return product;
    }

    /// <summary>
    /// Updates the discount of a product.
    /// Uses ExecuteUpdate for efficient bulk update without loading the entity.
    /// </summary>
    /// <param name="productId">Product ID.</param>
    /// <param name="discount">New discount percentage.</param>
    public async Task UpdateDiscountAsync(int productId, int discount)
    {
        await _context
            .Products.Where(p => p.Id == productId)
            .ExecuteUpdateAsync(s => s.SetProperty(p => p.Discount, discount));
    }

    /// <summary>
    /// Retrieves a product with change tracking enabled by its ID for administration.
    /// Used when modifications to the product entity are required.
    /// </summary>
    /// <param name="id">Product ID.</param>
    /// <returns>Tracked product entity or null if it doesn't exist.</returns>
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
