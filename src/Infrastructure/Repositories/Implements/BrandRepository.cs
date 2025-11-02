using Microsoft.EntityFrameworkCore;
using TiendaUcnApi.src.Domain.Models;
using TiendaUcnApi.src.Infrastructure.Data;
using TiendaUcnApi.src.Infrastructure.Repositories.Interfaces;

namespace TiendaUcnApi.src.Infrastructure.Repositories.Implements
{
    /// <summary>
    /// Implementation of the brand repository.
    /// Handles database operations for product brands.
    /// </summary>
    public class BrandRepository : IBrandRepository
    {
        private readonly AppDbContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="BrandRepository"/> class.
        /// </summary>
        /// <param name="context">Database context.</param>
        public BrandRepository(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Retrieves all brands with optional search and pagination.
        /// </summary>
        /// <param name="search">Optional search term to filter brands by name.</param>
        /// <param name="page">Page number.</param>
        /// <param name="size">Page size.</param>
        /// <returns>Collection of brands.</returns>
        public async Task<IEnumerable<Brand>> GetAllAsync(string? search, int page, int size)
        {
            var query = _context.Brands.AsQueryable();

            if (!string.IsNullOrEmpty(search))
                query = query.Where(b => b.Name.ToLower().Contains(search.ToLower()));

            return await query
                .OrderBy(b => b.Name)
                .Skip((page - 1) * size)
                .Take(size)
                .ToListAsync();
        }

        /// <summary>
        /// Retrieves a brand by its ID.
        /// </summary>
        /// <param name="id">Brand ID.</param>
        /// <returns>The brand if found, otherwise null.</returns>
        public async Task<Brand?> GetByIdAsync(int id)
        {
            return await _context.Brands.FindAsync(id);
        }

        /// <summary>
        /// Retrieves a brand by its name (case-insensitive).
        /// </summary>
        /// <param name="name">Brand name.</param>
        /// <returns>The brand if found, otherwise null.</returns>
        public async Task<Brand?> GetByNameAsync(string name)
        {
            return await _context.Brands.FirstOrDefaultAsync(b =>
                b.Name.ToLower() == name.ToLower()
            );
        }

        /// <summary>
        /// Creates a new brand.
        /// </summary>
        /// <param name="brand">The brand to create.</param>
        public async Task CreateAsync(Brand brand)
        {
            _context.Brands.Add(brand);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Updates an existing brand.
        /// </summary>
        /// <param name="brand">The brand to update.</param>
        public async Task UpdateAsync(Brand brand)
        {
            _context.Brands.Update(brand);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Deletes a brand.
        /// </summary>
        /// <param name="brand">The brand to delete.</param>
        public async Task DeleteAsync(Brand brand)
        {
            _context.Brands.Remove(brand);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Checks if a brand has associated products.
        /// </summary>
        /// <param name="brandId">Brand ID.</param>
        /// <returns>True if the brand has associated products, otherwise false.</returns>
        public async Task<bool> HasAssociatedProductsAsync(int brandId)
        {
            return await _context.Products.AnyAsync(p => p.BrandId == brandId);
        }
    }
}
