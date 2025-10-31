using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TiendaUcnApi.src.Application.DTO.CategoryDTO;
using TiendaUcnApi.src.Application.Services.Interfaces;
using TiendaUcnApi.src.Application.DTO.BaseResponse;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using TiendaUcnApi.src.Application.DTO;
using TiendaUcnApi.src.Application.DTO.AuthDTO;
using TiendaUcnApi.src.Application.Services.Interfaces;

namespace TiendaUcnApi.src.API.Controllers
{
    [ApiController]
    [Route("api/admin/categories")]
    [Authorize(Roles = "Administrador")]
    public class CategoryController : BaseController
    {
        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAll([FromQuery] string? search)
        {
            var categories = await _categoryService.GetAllAsync(search, 1, 100); 
            

            var data = new
            {
                categories = categories
            };

            return Ok(new GenericResponse<object>(
                "Categories retrieved successfully",
                data
            ));
        }

        
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var category = await _categoryService.GetByIdAsync(id);
            if (category == null)
                throw new KeyNotFoundException($"Category with ID {id} not found.");

            return Ok(new GenericResponse<CategoryDTO>(
                "Category retrieved successfully",
                category
            ));
        }

       
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CategoryCreateDTO dto)
        {
            var created = await _categoryService.CreateAsync(dto);
            if (created == null)
                throw new InvalidOperationException($"A category named '{dto.Name}' already exists.");

            return Ok(new GenericResponse<CategoryDTO>(
                "Category created successfully",
                created
            ));
        }

        
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] CategoryUpdateDTO dto)
        {
            var updated = await _categoryService.UpdateAsync(id, dto);
            if (updated == null)
                throw new KeyNotFoundException($"Category with ID {id} not found or name already in use.");

            return Ok(new GenericResponse<CategoryDTO>(
                "Category updated successfully",
                updated
            ));
        }

        
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _categoryService.DeleteAsync(id);
            if (!deleted)
                throw new InvalidOperationException("Cannot delete this category because it either does not exist or has associated products.");

            return Ok(new GenericResponse<string>(
                "Category deleted successfully",
                $"Category ID {id} removed."
            ));
        }
    }
}
