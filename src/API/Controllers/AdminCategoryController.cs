using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using TiendaUcnApi.src.Application.DTO;
using TiendaUcnApi.src.Application.DTO.AuthDTO;
using TiendaUcnApi.src.Application.DTO.BaseResponse;
using TiendaUcnApi.src.Application.DTO.CategoryDTO;
using TiendaUcnApi.src.Application.Services.Interfaces;

namespace TiendaUcnApi.src.API.Controllers
{
    /// <summary>
    /// Controller for category administration.
    /// Provides CRUD operations for product categories.
    /// Only accessible by users with "Administrador" role.
    /// </summary>
    [ApiController]
    [Route("api/admin/categories")]
    [Authorize(Roles = "Administrador")]
    public class CategoryController : BaseController
    {
        private readonly ICategoryService _categoryService;

        /// <summary>
        /// Initializes a new instance of the <see cref="CategoryController"/> class.
        /// </summary>
        /// <param name="categoryService">The category service for business logic.</param>
        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        /// <summary>
        /// Retrieves all categories with optional search filtering.
        /// Returns a maximum of 100 categories per request.
        /// </summary>
        /// <param name="search">Optional search term to filter categories by name.</param>
        /// <returns>List of categories.</returns>
        /// <response code="200">Returns the category list.</response>
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] string? search)
        {
            var categories = await _categoryService.GetAllAsync(search, 1, 100);

            var data = new { categories = categories };

            return Ok(new GenericResponse<object>("Categories retrieved successfully", data));
        }

        /// <summary>
        /// Retrieves a category by its identifier.
        /// </summary>
        /// <param name="id">The category identifier.</param>
        /// <returns>Category details.</returns>
        /// <response code="200">Returns the category details.</response>
        /// <response code="404">Category not found.</response>
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var category = await _categoryService.GetByIdAsync(id);
            if (category == null)
                throw new KeyNotFoundException($"Category with ID {id} not found.");

            return Ok(
                new GenericResponse<CategoryDTO>("Category retrieved successfully", category)
            );
        }

        /// <summary>
        /// Creates a new category.
        /// Validates that the category name does not already exist.
        /// </summary>
        /// <param name="dto">Category data to create.</param>
        /// <returns>Created category details.</returns>
        /// <response code="200">Category created successfully.</response>
        /// <response code="400">Invalid category data.</response>
        /// <response code="409">A category with the same name already exists.</response>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CategoryCreateDTO dto)
        {
            var created = await _categoryService.CreateAsync(dto);
            if (created == null)
                throw new InvalidOperationException(
                    $"A category named '{dto.Name}' already exists."
                );

            return Ok(new GenericResponse<CategoryDTO>("Category created successfully", created));
        }

        /// <summary>
        /// Updates an existing category.
        /// Validates that the new category name is not already in use by another category.
        /// </summary>
        /// <param name="id">The category identifier to update.</param>
        /// <param name="dto">Updated category data.</param>
        /// <returns>Updated category details.</returns>
        /// <response code="200">Category updated successfully.</response>
        /// <response code="404">Category not found or name already in use.</response>
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] CategoryUpdateDTO dto)
        {
            var updated = await _categoryService.UpdateAsync(id, dto);
            if (updated == null)
                throw new KeyNotFoundException(
                    $"Category with ID {id} not found or name already in use."
                );

            return Ok(new GenericResponse<CategoryDTO>("Category updated successfully", updated));
        }

        /// <summary>
        /// Deletes a category.
        /// Cannot delete categories that have associated products.
        /// </summary>
        /// <param name="id">The category identifier to delete.</param>
        /// <returns>Success message.</returns>
        /// <response code="200">Category deleted successfully.</response>
        /// <response code="404">Category not found.</response>
        /// <response code="409">Cannot delete category because it has associated products.</response>
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _categoryService.DeleteAsync(id);
            if (!deleted)
                throw new InvalidOperationException(
                    "Cannot delete this category because it either does not exist or has associated products."
                );

            return Ok(
                new GenericResponse<string>(
                    "Category deleted successfully",
                    $"Category ID {id} removed."
                )
            );
        }
    }
}
