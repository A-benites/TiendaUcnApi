using TiendaUcnApi.src.Application.DTO.BrandDTO;

namespace TiendaUcnApi.src.Application.Services.Interfaces
{
    public interface IBrandService
    {
        Task<IEnumerable<BrandDTO>> GetAllAsync(string? search, int page, int size);
        Task<BrandDTO?> GetByIdAsync(int id);
        Task<BrandDTO?> CreateAsync(BrandCreateDTO dto);
        Task<BrandDTO?> UpdateAsync(int id, BrandUpdateDTO dto);
        Task<bool> DeleteAsync(int id);
    }
}
