using Microsoft.AspNetCore.Mvc;
using TiendaUcnApi.src.Application.DTO;
using TiendaUcnApi.src.Application.DTO.ProductDTO;
using TiendaUcnApi.src.Application.Services.Interfaces;

namespace TiendaUcnApi.src.API.Controllers;

/// <summary>
/// Controller for public product operations accessible to all users.
/// Handles product listing and detail retrieval for customers.
/// </summary>
[ApiController]
[Route("api/products")]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;

    /// <summary>
    /// Initializes a new instance of the <see cref="ProductsController"/> class.
    /// </summary>
    /// <param name="productService">The product service for business logic.</param>
    public ProductsController(IProductService productService)
    {
        _productService = productService;
    }

    /// <summary>
    /// Retrieves a filtered and paginated list of products available to customers.
    /// </summary>
    /// <param name="searchParams">Search parameters including filters, sorting, and pagination.</param>
    /// <returns>Paginated list of products matching the search criteria.</returns>
    /// <response code="200">Returns the filtered product list.</response>
    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] SearchParamsDTO searchParams)
    {
        var response = await _productService.GetFilteredForCustomerAsync(searchParams);
        return Ok(response);
    }

    /// <summary>
    /// Retrieves detailed information for a specific product by ID.
    /// Only returns the product if it's available for purchase.
    /// </summary>
    /// <param name="id">The product identifier.</param>
    /// <returns>Product details if available.</returns>
    /// <response code="200">Returns the product details.</response>
    /// <response code="404">Product not found or not available.</response>
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
