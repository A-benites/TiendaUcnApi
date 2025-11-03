using TiendaUcnApi.src.Application.DTO.CategoryDTO;

namespace TiendaUcnApi.src.Application.Services.Interfaces
{
    /// <summary>
    /// Service interface for product category management operations.
    /// </summary>
    public interface ICategoryService
    {
        /// <summary>
        /// Retrieves all categories with optional search and pagination.
        /// </summary>
        /// <param name="search">Optional search term to filter categories by name.</param>
        /// <param name="page">Page number (1-indexed).</param>
        /// <param name="size">Number of categories per page.</param>
        /// <returns>Collection of category DTOs.</returns>
        Task<IEnumerable<CategoryDTO>> GetAllAsync(string? search, int page, int size);

        /// <summary>
        /// Retrieves a specific category by its ID.
        /// </summary>
        /// <param name="id">The category identifier.</param>
        /// <returns>Category DTO or null if not found.</returns>
        Task<CategoryDTO?> GetByIdAsync(int id);

        /// <summary>
        /// Creates a new category in the system.
        /// </summary>
        /// <param name="dto">DTO containing the category data to create.</param>
        /// <returns>Created category DTO or null if creation failed.</returns>
        Task<CategoryDTO?> CreateAsync(CategoryCreateDTO dto);

        /// <summary>
        /// Updates an existing category's information.
        /// </summary>
        /// <param name="id">The category identifier to update.</param>
        /// <param name="dto">DTO containing updated category data.</param>
        /// <returns>Updated category DTO or null if not found.</returns>
        Task<CategoryDTO?> UpdateAsync(int id, CategoryUpdateDTO dto);

        /// <summary>
        /// Deletes a category from the system.
        /// </summary>
        /// <param name="id">The category identifier to delete.</param>
        /// <returns>True if deletion was successful, false otherwise.</returns>
        Task<bool> DeleteAsync(int id);
    }
}
