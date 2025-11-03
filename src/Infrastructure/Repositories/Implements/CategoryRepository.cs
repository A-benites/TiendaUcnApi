using Microsoft.EntityFrameworkCore;
using TiendaUcnApi.src.Domain.Models;
using TiendaUcnApi.src.Infrastructure.Data;
using TiendaUcnApi.src.Infrastructure.Repositories.Interfaces;

namespace TiendaUcnApi.src.Infrastructure.Repositories.Implements
{
    /// <summary>
    /// Implementation of the category repository.
    /// Handles database operations for product categories.
    /// </summary>
    public class CategoryRepository : ICategoryRepository
    {
        private readonly AppDbContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="CategoryRepository"/> class.
        /// </summary>
        /// <param name="context">Database context.</param>
        public CategoryRepository(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Retrieves all categories with optional search and pagination.
        /// </summary>
        /// <param name="search">Optional search term to filter categories by name.</param>
        /// <param name="page">Page number.</param>
        /// <param name="size">Page size.</param>
        /// <returns>Collection of categories with associated products.</returns>
        public async Task<IEnumerable<Category>> GetAllAsync(string? search, int page, int size)
        {
            var query = _context.Categories.Include(c => c.Products).AsQueryable();

            if (!string.IsNullOrEmpty(search))
                query = query.Where(c => c.Name.ToLower().Contains(search.ToLower()));

            return await query
                .OrderBy(c => c.Name)
                .Skip((page - 1) * size)
                .Take(size)
                .ToListAsync();
        }

        /// <summary>
        /// Retrieves a category by its ID.
        /// </summary>
        /// <param name="id">Category ID.</param>
        /// <returns>The category with associated products, or null if not found.</returns>
        public async Task<Category?> GetByIdAsync(int id)
        {
            return await _context
                .Categories.Include(c => c.Products)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        /// <summary>
        /// Retrieves a category by its name (case-insensitive).
        /// </summary>
        /// <param name="name">Category name.</param>
        /// <returns>The category if found, otherwise null.</returns>
        public async Task<Category?> GetByNameAsync(string name)
        {
            return await _context.Categories.FirstOrDefaultAsync(c =>
                c.Name.ToLower() == name.ToLower()
            );
        }

        /// <summary>
        /// Creates a new category.
        /// </summary>
        /// <param name="category">The category to create.</param>
        public async Task CreateAsync(Category category)
        {
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Updates an existing category.
        /// </summary>
        /// <param name="category">The category to update.</param>
        public async Task UpdateAsync(Category category)
        {
            _context.Categories.Update(category);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Deletes a category.
        /// </summary>
        /// <param name="category">The category to delete.</param>
        public async Task DeleteAsync(Category category)
        {
            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Retrieves all shopping carts with related data.
        /// Note: This method seems misplaced in CategoryRepository and should likely be in CartRepository.
        /// </summary>
        /// <returns>List of all carts with items and user data.</returns>
        public async Task<List<Cart>> GetAllAsync()
        {
            return await _context
                .Carts.Include(c => c.CartItems) // adjust name if your property is different
                .ThenInclude(ci => ci.Product) // optional: include product data
                .Include(c => c.User) // include user navigation if exists
                .ToListAsync();
        }
    }
}
