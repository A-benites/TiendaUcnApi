using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TiendaUcnApi.src.Application.DTO;
using TiendaUcnApi.src.Application.DTO.BrandDTO;
using TiendaUcnApi.src.Application.Services.Interfaces;

namespace TiendaUcnApi.src.API.Controllers.Admin
{
    /// <summary>
    /// Controller for brand administration.
    /// Provides CRUD operations for product brands.
    /// Only accessible by users with "Administrador" role.
    /// </summary>
    [Authorize(Roles = "Administrador")]
    [Route("api/admin/brands")]
    public class BrandController : BaseController
    {
        private readonly IBrandService _brandService;

        /// <summary>
        /// Initializes a new instance of the <see cref="BrandController"/> class.
        /// </summary>
        /// <param name="brandService">The brand service for business logic.</param>
        public BrandController(IBrandService brandService)
        {
            _brandService = brandService;
        }

        /// <summary>
        /// Retrieves all brands with optional search filtering and pagination.
        /// </summary>
        /// <param name="search">Optional search term to filter brands by name.</param>
        /// <param name="page">Page number (default: 1, must be greater than 0).</param>
        /// <param name="size">Page size (default: 10, must be greater than 0).</param>
        /// <returns>Paginated list of brands.</returns>
        /// <response code="200">Returns the brand list.</response>
        /// <response code="400">Invalid page or size parameters.</response>
        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] string? search,
            [FromQuery] int page = 1,
            [FromQuery] int size = 10
        )
        {
            if (page <= 0)
                throw new ArgumentOutOfRangeException(nameof(page), "Page must be greater than 0.");
            if (size <= 0)
                throw new ArgumentOutOfRangeException(nameof(size), "Size must be greater than 0.");

            // Fixed R111: Call _brandService instead of _categoryService
            var result = await _brandService.GetAllAsync(search, page, size);

            return Ok(new GenericResponse<object>("Brands retrieved successfully", result));
        }

        /// <summary>
        /// Retrieves a brand by its identifier.
        /// </summary>
        /// <param name="id">The brand identifier.</param>
        /// <returns>Brand details.</returns>
        /// <response code="200">Returns the brand details.</response>
        /// <response code="404">Brand not found.</response>
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result =
                await _brandService.GetByIdAsync(id)
                ?? throw new KeyNotFoundException("Brand not found.");

            return Ok(new GenericResponse<object>("Brand retrieved successfully", result));
        }

        /// <summary>
        /// Creates a new brand.
        /// Validates that the brand name does not already exist.
        /// </summary>
        /// <param name="dto">Brand data to create.</param>
        /// <returns>Created brand details.</returns>
        /// <response code="201">Brand created successfully.</response>
        /// <response code="400">Invalid brand data.</response>
        /// <response code="409">A brand with the same name already exists.</response>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] BrandCreateDTO dto)
        {
            var result =
                await _brandService.CreateAsync(dto)
                ?? throw new InvalidOperationException(
                    "A brand with the same name already exists."
                );

            return CreatedAtAction(
                nameof(GetById),
                new { id = result.Id },
                new GenericResponse<object>("Brand created successfully", result)
            );
        }

        /// <summary>
        /// Updates an existing brand.
        /// Validates that the new brand name is not already in use by another brand.
        /// </summary>
        /// <param name="id">The brand identifier to update.</param>
        /// <param name="dto">Updated brand data.</param>
        /// <returns>Updated brand details.</returns>
        /// <response code="200">Brand updated successfully.</response>
        /// <response code="404">Brand not found.</response>
        /// <response code="409">Brand name already in use.</response>
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] BrandUpdateDTO dto)
        {
            var result =
                await _brandService.UpdateAsync(id, dto)
                ?? throw new InvalidOperationException("Brand name already in use or not found.");

            return Ok(new GenericResponse<object>("Brand updated successfully", result));
        }

        /// <summary>
        /// Deletes a brand.
        /// Cannot delete brands that have associated products.
        /// </summary>
        /// <param name="id">The brand identifier to delete.</param>
        /// <returns>Success message.</returns>
        /// <response code="200">Brand deleted successfully.</response>
        /// <response code="404">Brand not found.</response>
        /// <response code="409">Cannot delete brand because it has associated products.</response>
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _brandService.DeleteAsync(id);

            if (!success)
                throw new InvalidOperationException(
                    "Cannot delete brand because it has associated products or was not found."
                );

            return Ok(new GenericResponse<object>("Brand deleted successfully", null));
        }
    }
}
