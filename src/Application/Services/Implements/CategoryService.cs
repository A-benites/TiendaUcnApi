using TiendaUcnApi.src.Application.DTO.CategoryDTO;
using TiendaUcnApi.src.Application.Services.Interfaces;
using TiendaUcnApi.src.Domain.Models;
using TiendaUcnApi.src.Infrastructure.Repositories.Interfaces;

namespace TiendaUcnApi.src.Application.Services.Implements
{
    /// <summary>
    /// Service implementation for managing product categories.
    /// </summary>
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _repository;

        /// <summary>
        /// Initializes a new instance of the CategoryService class.
        /// </summary>
        /// <param name="repository">Category repository.</param>
        public CategoryService(ICategoryRepository repository)
        {
            _repository = repository;
        }

        /// <summary>
        /// Retrieves all categories with optional search filtering and pagination.
        /// </summary>
        /// <param name="search">Optional search term to filter categories by name.</param>
        /// <param name="page">Page number (1-based).</param>
        /// <param name="size">Number of items per page.</param>
        /// <returns>List of categories with product counts.</returns>
        public async Task<IEnumerable<CategoryDTO>> GetAllAsync(string? search, int page, int size)
        {
            var categories = await _repository.GetAllAsync(search, page, size);

            return categories.Select(c => new CategoryDTO
            {
                Id = c.Id,
                Name = c.Name,
                CreatedAt = c.CreatedAt,
                ProductCount = c.Products.Count,
            });
        }

        /// <summary>
        /// Retrieves a category by its ID.
        /// </summary>
        /// <param name="id">Category ID.</param>
        /// <returns>Category data or null if not found.</returns>
        public async Task<CategoryDTO?> GetByIdAsync(int id)
        {
            var category = await _repository.GetByIdAsync(id);
            if (category == null)
                return null;

            return new CategoryDTO
            {
                Id = category.Id,
                Name = category.Name,
                CreatedAt = category.CreatedAt,
                ProductCount = category.Products.Count,
            };
        }

        /// <summary>
        /// Creates a new category.
        /// Validates that the category name is unique (case-insensitive).
        /// </summary>
        /// <param name="dto">Category creation data.</param>
        /// <returns>Created category or null if name already exists.</returns>
        public async Task<CategoryDTO?> CreateAsync(CategoryCreateDTO dto)
        {
            // Verify duplicate name
            var existing = await _repository.GetByNameAsync(dto.Name);
            if (existing != null)
                return null;

            var category = new Category { Name = dto.Name.Trim(), CreatedAt = DateTime.UtcNow };

            await _repository.CreateAsync(category);

            return new CategoryDTO
            {
                Id = category.Id,
                Name = category.Name,
                CreatedAt = category.CreatedAt,
                ProductCount = 0,
            };
        }

        /// <summary>
        /// Updates an existing category.
        /// Validates that the new name doesn't conflict with other categories.
        /// </summary>
        /// <param name="id">Category ID to update.</param>
        /// <param name="dto">Updated category data.</param>
        /// <returns>Updated category or null if not found or name already exists.</returns>
        public async Task<CategoryDTO?> UpdateAsync(int id, CategoryUpdateDTO dto)
        {
            var category = await _repository.GetByIdAsync(id);
            if (category == null)
                return null;

            // Avoid duplicate names
            var duplicate = await _repository.GetByNameAsync(dto.Name);
            if (duplicate != null && duplicate.Id != id)
                return null;

            category.Name = dto.Name.Trim();

            await _repository.UpdateAsync(category);

            return new CategoryDTO
            {
                Id = category.Id,
                Name = category.Name,
                CreatedAt = category.CreatedAt,
                ProductCount = category.Products.Count,
            };
        }

        /// <summary>
        /// Deletes a category.
        /// Implements R111 rubric requirement: prevents deletion if category has associated products.
        /// </summary>
        /// <param name="id">Category ID to delete.</param>
        /// <returns>True if deletion was successful, false if category not found or has associated products.</returns>
        public async Task<bool> DeleteAsync(int id)
        {
            var category = await _repository.GetByIdAsync(id);
            if (category == null)
                return false;

            // Validate referential integrity (associated products)
            if (category.Products.Any())
                return false;

            await _repository.DeleteAsync(category);
            return true;
        }
    }
}
