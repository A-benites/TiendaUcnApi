using TiendaUcnApi.src.Application.DTO.BrandDTO;
using TiendaUcnApi.src.Application.Exceptions;
using TiendaUcnApi.src.Application.Services.Interfaces;
using TiendaUcnApi.src.Domain.Models;
using TiendaUcnApi.src.Infrastructure.Repositories.Interfaces;

namespace TiendaUcnApi.src.Application.Services.Implements
{
    public class BrandService : IBrandService
    {
        private readonly IBrandRepository _repository;

        public BrandService(IBrandRepository repository)
        {
            _repository = repository;
        }

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

        public async Task<BrandDTO?> CreateAsync(BrandCreateDTO dto)
        {
            // Validar nombre duplicado
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
                    "No se puede eliminar la marca porque tiene productos asociados."
                );
            }

            await _repository.DeleteAsync(brand);
            return true;
        }
    }
}
