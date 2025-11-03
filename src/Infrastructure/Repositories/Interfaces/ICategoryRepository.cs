using TiendaUcnApi.src.Domain.Models;

namespace TiendaUcnApi.src.Infrastructure.Repositories.Interfaces
{
    /// <summary>
    /// Repository interface for product category data access operations.
    /// Provides methods for CRUD operations and category-specific queries.
    /// </summary>
    public interface ICategoryRepository
    {
        /// <summary>
        /// Retrieves all categories with optional search filtering and pagination.
        /// </summary>
        /// <param name="search">Optional search term to filter categories by name.</param>
        /// <param name="page">Page number (1-indexed).</param>
        /// <param name="size">Number of categories per page.</param>
        /// <returns>Collection of categories matching the search criteria.</returns>
        Task<IEnumerable<Category>> GetAllAsync(string? search, int page, int size);

        /// <summary>
        /// Retrieves a specific category by its identifier.
        /// </summary>
        /// <param name="id">The category identifier.</param>
        /// <returns>Category entity or null if not found.</returns>
        Task<Category?> GetByIdAsync(int id);

        /// <summary>
        /// Retrieves a specific category by its name.
        /// </summary>
        /// <param name="name">The category name to search for.</param>
        /// <returns>Category entity or null if not found.</returns>
        Task<Category?> GetByNameAsync(string name);

        /// <summary>
        /// Creates a new category in the database.
        /// </summary>
        /// <param name="category">The category entity to create.</param>
        Task CreateAsync(Category category);

        /// <summary>
        /// Updates an existing category in the database.
        /// </summary>
        /// <param name="category">The category entity with updated data.</param>
        Task UpdateAsync(Category category);

        /// <summary>
        /// Deletes a category from the database.
        /// </summary>
        /// <param name="category">The category entity to delete.</param>
        Task DeleteAsync(Category category);
    }
}
