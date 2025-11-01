using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TiendaUcnApi.src.Application.DTO;
using TiendaUcnApi.src.Application.DTO.BrandDTO;
using TiendaUcnApi.src.Application.Services.Interfaces;

namespace TiendaUcnApi.src.API.Controllers.Admin
{
    
    [Authorize(Roles = "Administrador")]
    [Route("api/admin/brands")]
    public class BrandController : BaseController
    {
        private readonly IBrandService _brandService;
        private readonly ICategoryService _categoryService;

        public BrandController(IBrandService brandService,ICategoryService cartegoryService)
        {
            _brandService = brandService;

            _categoryService = cartegoryService;

        }

        
        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] string? search,
            [FromQuery] int page = 1,
            [FromQuery] int size = 10)
        {
            
            if (page <= 0) throw new ArgumentOutOfRangeException(nameof(page), "Page must be greater than 0.");
            if (size <= 0) throw new ArgumentOutOfRangeException(nameof(size), "Size must be greater than 0.");

            
            var result = await _categoryService.GetAllAsync(search, page, size);

           
            return Ok(new GenericResponse<object>("Categories retrieved successfully", result));
        }

        
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _brandService.GetByIdAsync(id)
                ?? throw new KeyNotFoundException("Brand not found.");

            return Ok(new GenericResponse<object>("Brand retrieved successfully", result));
        }

       
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] BrandCreateDTO dto)
        {
            var result = await _brandService.CreateAsync(dto)
                ?? throw new InvalidOperationException("A brand with the same name already exists.");

            return CreatedAtAction(nameof(GetById), new { id = result.Id },
                new GenericResponse<object>("Brand created successfully", result));
        }

        
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] BrandUpdateDTO dto)
        {
            var result = await _brandService.UpdateAsync(id, dto)
                ?? throw new InvalidOperationException("Brand name already in use or not found.");

            return Ok(new GenericResponse<object>("Brand updated successfully", result));
        }

        
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _brandService.DeleteAsync(id);

            if (!success)
                throw new InvalidOperationException("Cannot delete brand because it has associated products or was not found.");

            return Ok(new GenericResponse<object>("Brand deleted successfully", null));
        }
    }
}
