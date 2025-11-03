using TiendaUcnApi.src.Application.DTO.BrandDTO;

namespace TiendaUcnApi.src.Application.Services.Interfaces
{
    /// <summary>
    /// Service interface for brand management operations.
    /// </summary>
    public interface IBrandService
    {
        /// <summary>
        /// Retrieves all brands with optional search and pagination.
        /// </summary>
        /// <param name="search">Optional search term to filter brands by name.</param>
        /// <param name="page">Page number (1-indexed).</param>
        /// <param name="size">Number of brands per page.</param>
        /// <returns>Collection of brand DTOs.</returns>
        Task<IEnumerable<BrandDTO>> GetAllAsync(string? search, int page, int size);

        /// <summary>
        /// Retrieves a specific brand by its ID.
        /// </summary>
        /// <param name="id">The brand identifier.</param>
        /// <returns>Brand DTO or null if not found.</returns>
        Task<BrandDTO?> GetByIdAsync(int id);

        /// <summary>
        /// Creates a new brand in the system.
        /// </summary>
        /// <param name="dto">DTO containing the brand data to create.</param>
        /// <returns>Created brand DTO or null if creation failed.</returns>
        Task<BrandDTO?> CreateAsync(BrandCreateDTO dto);

        /// <summary>
        /// Updates an existing brand's information.
        /// </summary>
        /// <param name="id">The brand identifier to update.</param>
        /// <param name="dto">DTO containing updated brand data.</param>
        /// <returns>Updated brand DTO or null if not found.</returns>
        Task<BrandDTO?> UpdateAsync(int id, BrandUpdateDTO dto);

        /// <summary>
        /// Deletes a brand from the system.
        /// </summary>
        /// <param name="id">The brand identifier to delete.</param>
        /// <returns>True if deletion was successful, false otherwise.</returns>
        Task<bool> DeleteAsync(int id);
    }
}
