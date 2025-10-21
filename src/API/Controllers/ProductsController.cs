using Microsoft.AspNetCore.Mvc;
using TiendaUcnApi.src.Application.DTO;
using TiendaUcnApi.src.Application.DTO.ProductDTO;
using TiendaUcnApi.src.Application.Services.Interfaces;

namespace TiendaUcnApi.src.API.Controllers;

[ApiController]
[Route("api/products")]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;

    public ProductsController(IProductService productService)
    {
        _productService = productService;
    }

    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] SearchParamsDTO searchParams)
    {
        var response = await _productService.GetFilteredForCustomerAsync(searchParams);
        return Ok(response);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var response = await _productService.GetByIdForCustomerAsync(id);
        if (response.Data == null)
        {
            return NotFound(response);
        }
        return Ok(response);
    }
}
