using TiendaUcnApi.src.Application.DTO.CategoryDTO;

namespace TiendaUcnApi.src.Application.Services.Interfaces
{
    public interface ICategoryService
    {
        Task<IEnumerable<CategoryDTO>> GetAllAsync(string? search, int page, int size);
        Task<CategoryDTO?> GetByIdAsync(int id);
        Task<CategoryDTO?> CreateAsync(CategoryCreateDTO dto);
        Task<CategoryDTO?> UpdateAsync(int id, CategoryUpdateDTO dto);
        Task<bool> DeleteAsync(int id);
    }
}
