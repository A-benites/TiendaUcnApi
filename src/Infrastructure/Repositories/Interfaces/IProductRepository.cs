using TiendaUcnApi.src.Application.DTO.ProductDTO;
using TiendaUcnApi.src.Domain.Models;

namespace TiendaUcnApi.src.Infrastructure.Repositories.Interfaces;

/// <summary>
/// Repository interface for product data access operations.
/// Handles product CRUD operations, filtering, and stock management.
/// </summary>
public interface IProductRepository
{
    /// <summary>
    /// Retrieves filtered products for admin view with pagination and search criteria.
    /// </summary>
    /// <param name="searchParams">Search parameters including filters, sorting, and pagination.</param>
    /// <returns>Tuple containing products collection and total count.</returns>
    Task<(IEnumerable<Product> products, int totalCount)> GetFilteredForAdminAsync(
        SearchParamsDTO searchParams
    );

    /// <summary>
    /// Retrieves filtered products for customer view with pagination and search criteria.
    /// Only returns available products.
    /// </summary>
    /// <param name="searchParams">Search parameters including filters, sorting, and pagination.</param>
    /// <returns>Tuple containing products collection and total count.</returns>
    Task<(IEnumerable<Product> products, int totalCount)> GetFilteredForCustomerAsync(
        SearchParamsDTO searchParams
    );

    /// <summary>
    /// Retrieves a specific product by its identifier.
    /// </summary>
    /// <param name="id">The product identifier.</param>
    /// <returns>Product entity or null if not found.</returns>
    Task<Product?> GetByIdAsync(int id);

    /// <summary>
    /// Creates a new product in the database.
    /// </summary>
    /// <param name="product">The product entity to create.</param>
    /// <returns>The identifier of the created product.</returns>
    Task<int> CreateAsync(Product product);

    /// <summary>
    /// Retrieves a category by its identifier.
    /// </summary>
    /// <param name="id">The category identifier.</param>
    /// <returns>Category entity or null if not found.</returns>
    Task<Category?> GetCategoryByIdAsync(int id);

    /// <summary>
    /// Retrieves a brand by its identifier.
    /// </summary>
    /// <param name="id">The brand identifier.</param>
    /// <returns>Brand entity or null if not found.</returns>
    Task<Brand?> GetBrandByIdAsync(int id);

    /// <summary>
    /// Toggles the availability status of a product (soft delete).
    /// </summary>
    /// <param name="id">The product identifier whose status will be toggled.</param>
    Task ToggleActiveAsync(int id);

    /// <summary>
    /// Gets the actual stock quantity for a product.
    /// </summary>
    /// <param name="productId">The product identifier.</param>
    /// <returns>Current stock quantity.</returns>
    Task<int> GetRealStockAsync(int productId);

    /// <summary>
    /// Updates the stock quantity of a product.
    /// </summary>
    /// <param name="productId">The product identifier.</param>
    /// <param name="stock">New stock quantity.</param>
    Task UpdateStockAsync(int productId, int stock);

    /// <summary>
    /// Retrieves a product by its identifier for admin view.
    /// Includes all product data regardless of availability status.
    /// </summary>
    /// <param name="id">The product identifier.</param>
    /// <returns>Product entity or null if not found.</returns>
    Task<Product?> GetByIdForAdminAsync(int id);

    /// <summary>
    /// Retrieves a product by its identifier for customer view.
    /// Only returns the product if it's available.
    /// </summary>
    /// <param name="id">The product identifier.</param>
    /// <returns>Product entity or null if not found or unavailable.</returns>
    Task<Product?> GetByIdForCustomerAsync(int id);

    /// <summary>
    /// Updates an existing product in the database.
    /// </summary>
    /// <param name="product">The product entity with updated data.</param>
    /// <returns>The updated product entity.</returns>
    Task<Product> UpdateAsync(Product product);

    /// <summary>
    /// Updates the discount percentage of a product.
    /// </summary>
    /// <param name="productId">The product identifier.</param>
    /// <param name="discount">New discount percentage (0-100).</param>
    Task UpdateDiscountAsync(int productId, int discount);

    /// <summary>
    /// Retrieves a product with change tracking enabled for admin operations.
    /// Used when modifications need to be tracked by Entity Framework.
    /// </summary>
    /// <param name="id">The product identifier.</param>
    /// <returns>Tracked product entity or null if not found.</returns>
    Task<Product?> GetTrackedByIdForAdminAsync(int id);
}
