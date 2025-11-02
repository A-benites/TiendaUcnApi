using TiendaUcnApi.src.Application.DTO.BrandDTO;
using TiendaUcnApi.src.Application.Exceptions;
using TiendaUcnApi.src.Application.Services.Interfaces;
using TiendaUcnApi.src.Domain.Models;
using TiendaUcnApi.src.Infrastructure.Repositories.Interfaces;

namespace TiendaUcnApi.src.Application.Services.Implements
{
    /// <summary>
    /// Service for managing product brands.
    /// Provides CRUD operations with duplicate name validation and product association checks.
    /// </summary>
    public class BrandService : IBrandService
    {
        /// <summary>
        /// Brand repository for data access.
        /// </summary>
        private readonly IBrandRepository _repository;

        /// <summary>
        /// Initializes a new instance of the BrandService class.
        /// </summary>
        /// <param name="repository">Brand repository.</param>
        public BrandService(IBrandRepository repository)
        {
            _repository = repository;
        }

        /// <summary>
        /// Retrieves all brands with optional search filtering and pagination.
        /// </summary>
        /// <param name="search">Optional search term to filter brands by name.</param>
        /// <param name="page">Page number for pagination.</param>
        /// <param name="size">Page size for pagination.</param>
        /// <returns>Collection of brand DTOs.</returns>
        public async Task<IEnumerable<BrandDTO>> GetAllAsync(string? search, int page, int size)
        {
            var brands = await _repository.GetAllAsync(search, page, size);

            return brands.Select(b => new BrandDTO
            {
                Id = b.Id,
                Name = b.Name,
                CreatedAt = b.CreatedAt,
            });
        }

        /// <summary>
        /// Retrieves a brand by its ID.
        /// </summary>
        /// <param name="id">Brand ID.</param>
        /// <returns>Brand DTO if found, null otherwise.</returns>
        public async Task<BrandDTO?> GetByIdAsync(int id)
        {
            var brand = await _repository.GetByIdAsync(id);
            if (brand == null)
                return null;

            return new BrandDTO
            {
                Id = brand.Id,
                Name = brand.Name,
                CreatedAt = brand.CreatedAt,
            };
        }

        /// <summary>
        /// Creates a new brand.
        /// Validates that the brand name is unique before creation.
        /// </summary>
        /// <param name="dto">DTO containing brand creation data.</param>
        /// <returns>Created brand DTO if successful, null if a brand with the same name already exists.</returns>
        public async Task<BrandDTO?> CreateAsync(BrandCreateDTO dto)
        {
            // Validate duplicate name
            var existing = await _repository.GetByNameAsync(dto.Name);
            if (existing != null)
                return null;

            var brand = new Brand { Name = dto.Name.Trim(), CreatedAt = DateTime.UtcNow };

            await _repository.CreateAsync(brand);

            return new BrandDTO
            {
                Id = brand.Id,
                Name = brand.Name,
                CreatedAt = brand.CreatedAt,
            };
        }

        /// <summary>
        /// Updates an existing brand.
        /// Validates that the new name doesn't conflict with existing brands (except itself).
        /// </summary>
        /// <param name="id">ID of the brand to update.</param>
        /// <param name="dto">DTO containing updated brand data.</param>
        /// <returns>Updated brand DTO if successful, null if brand not found or name conflict exists.</returns>
        public async Task<BrandDTO?> UpdateAsync(int id, BrandUpdateDTO dto)
        {
            var brand = await _repository.GetByIdAsync(id);
            if (brand == null)
                return null;

            var duplicate = await _repository.GetByNameAsync(dto.Name);
            if (duplicate != null && duplicate.Id != id)
                return null;

            brand.Name = dto.Name.Trim();

            await _repository.UpdateAsync(brand);

            return new BrandDTO
            {
                Id = brand.Id,
                Name = brand.Name,
                CreatedAt = brand.CreatedAt,
            };
        }

        /// <summary>
        /// Deletes a brand by its ID.
        /// Implements R110: Prevents deletion if the brand has associated products.
        /// </summary>
        /// <param name="id">ID of the brand to delete.</param>
        /// <returns>True if deletion was successful, false if brand not found.</returns>
        /// <exception cref="ConflictException">Thrown when the brand has associated products.</exception>
        public async Task<bool> DeleteAsync(int id)
        {
            var brand = await _repository.GetByIdAsync(id);
            if (brand == null)
                return false;

            // R110: Verify if brand has associated products before deletion
            var hasProducts = await _repository.HasAssociatedProductsAsync(id);
            if (hasProducts)
            {
                throw new ConflictException(
                    "Cannot delete the brand because it has associated products."
                );
            }

            await _repository.DeleteAsync(brand);
            return true;
        }
    }
}
