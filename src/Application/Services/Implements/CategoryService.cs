using TiendaUcnApi.src.Application.DTO.CategoryDTO;
using TiendaUcnApi.src.Application.Services.Interfaces;
using TiendaUcnApi.src.Domain.Models;
using TiendaUcnApi.src.Infrastructure.Repositories.Interfaces;

namespace TiendaUcnApi.src.Application.Services.Implements
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _repository;

        public CategoryService(ICategoryRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<CategoryDTO>> GetAllAsync(string? search, int page, int size)
        {
            var categories = await _repository.GetAllAsync(search, page, size);

            return categories.Select(c => new CategoryDTO
            {
                Id = c.Id,
                Name = c.Name,
                CreatedAt = c.CreatedAt,
                ProductCount = c.Products.Count
            });
        }

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
                ProductCount = category.Products.Count
            };
        }

        public async Task<CategoryDTO?> CreateAsync(CategoryCreateDTO dto)
        {
            // Verificar nombre duplicado
            var existing = await _repository.GetByNameAsync(dto.Name);
            if (existing != null)
                return null;

            var category = new Category
            {
                Name = dto.Name.Trim(),
                CreatedAt = DateTime.UtcNow
            };

            await _repository.CreateAsync(category);

            return new CategoryDTO
            {
                Id = category.Id,
                Name = category.Name,
                CreatedAt = category.CreatedAt,
                ProductCount = 0
            };
        }

        public async Task<CategoryDTO?> UpdateAsync(int id, CategoryUpdateDTO dto)
        {
            var category = await _repository.GetByIdAsync(id);
            if (category == null)
                return null;

            // Evitar nombres duplicados
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
                ProductCount = category.Products.Count
            };
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var category = await _repository.GetByIdAsync(id);
            if (category == null)
                return false;

            // Validar integridad referencial (productos asociados)
            if (category.Products.Any())
                return false;

            await _repository.DeleteAsync(category);
            return true;
        }
    }
}
