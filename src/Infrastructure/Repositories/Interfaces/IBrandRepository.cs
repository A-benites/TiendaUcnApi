using TiendaUcnApi.src.Domain.Models;

namespace TiendaUcnApi.src.Infrastructure.Repositories.Interfaces
{
    /// <summary>
    /// Repository interface for brand data access operations.
    /// Provides methods for CRUD operations and brand-specific queries.
    /// </summary>
    public interface IBrandRepository
    {
        /// <summary>
        /// Retrieves all brands with optional search filtering and pagination.
        /// </summary>
        /// <param name="search">Optional search term to filter brands by name.</param>
        /// <param name="page">Page number (1-indexed).</param>
        /// <param name="size">Number of brands per page.</param>
        /// <returns>Collection of brands matching the search criteria.</returns>
        Task<IEnumerable<Brand>> GetAllAsync(string? search, int page, int size);

        /// <summary>
        /// Retrieves a specific brand by its identifier.
        /// </summary>
        /// <param name="id">The brand identifier.</param>
        /// <returns>Brand entity or null if not found.</returns>
        Task<Brand?> GetByIdAsync(int id);

        /// <summary>
        /// Retrieves a specific brand by its name.
        /// </summary>
        /// <param name="name">The brand name to search for.</param>
        /// <returns>Brand entity or null if not found.</returns>
        Task<Brand?> GetByNameAsync(string name);

        /// <summary>
        /// Creates a new brand in the database.
        /// </summary>
        /// <param name="brand">The brand entity to create.</param>
        Task CreateAsync(Brand brand);

        /// <summary>
        /// Updates an existing brand in the database.
        /// </summary>
        /// <param name="brand">The brand entity with updated data.</param>
        Task UpdateAsync(Brand brand);

        /// <summary>
        /// Deletes a brand from the database.
        /// </summary>
        /// <param name="brand">The brand entity to delete.</param>
        Task DeleteAsync(Brand brand);

        /// <summary>
        /// Checks if a brand has any associated products.
        /// </summary>
        /// <param name="brandId">The brand identifier to check.</param>
        /// <returns>True if the brand has associated products, false otherwise.</returns>
        Task<bool> HasAssociatedProductsAsync(int brandId);
    }
}
