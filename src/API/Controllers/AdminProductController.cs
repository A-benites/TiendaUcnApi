using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TiendaUcnApi.src.Application.DTO;
using TiendaUcnApi.src.Application.DTO.ProductDTO;
using TiendaUcnApi.src.Application.DTO.ProductDTO.AdminDTO;
using TiendaUcnApi.src.Application.Services.Interfaces;

namespace TiendaUcnApi.src.API.Controllers;

/// <summary>
/// Controller for product administration operations.
/// Allows CRUD operations, image management, and discount management.
/// Only accessible by users with "Administrador" role.
/// </summary>
[ApiController]
[Route("api/admin/products")] // Base route adjusted for all admin endpoints
[Authorize(Roles = "Administrador")] // Protection at controller level
public class AdminProductController : ControllerBase // Changed to avoid route conflict
{
    private readonly IProductService _productService;
    private readonly IFileService _fileService;

    /// <summary>
    /// Initializes a new instance of the <see cref="AdminProductController"/> class.
    /// </summary>
    /// <param name="productService">The product service for business logic.</param>
    /// <param name="fileService">The file service for image management.</param>
    public AdminProductController(IProductService productService, IFileService fileService)
    {
        _productService = productService;
        _fileService = fileService;
    }

    /// <summary>
    /// Retrieves all products for administration with filters and pagination.
    /// </summary>
    /// <param name="searchParams">Search parameters including filters, sorting, and pagination.</param>
    /// <returns>Filtered and paginated product list.</returns>
    /// <response code="200">Returns the product list.</response>
    [HttpGet]
    public async Task<IActionResult> GetAllForAdminAsync([FromQuery] SearchParamsDTO searchParams)
    {
        var result = await _productService.GetFilteredForAdminAsync(searchParams);
        return Ok(
            new GenericResponse<ListedProductsForAdminDTO>(
                "Products retrieved successfully",
                result
            )
        );
    }

    /// <summary>
    /// Retrieves detailed information for a specific product by ID for administration.
    /// Includes all product data regardless of availability status.
    /// </summary>
    /// <param name="id">The product identifier.</param>
    /// <returns>Product details.</returns>
    /// <response code="200">Returns the product details.</response>
    /// <response code="404">Product not found.</response>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetByIdForAdminAsync(int id)
    {
        var result = await _productService.GetByIdForAdminAsync(id);
        return Ok(new GenericResponse<ProductDetailDTO>("Product retrieved successfully", result));
    }

    /// <summary>
    /// Creates a new product with images.
    /// </summary>
    /// <param name="createProductDTO">Product data including images to create.</param>
    /// <returns>Created product ID.</returns>
    /// <response code="201">Product created successfully.</response>
    /// <response code="400">Invalid product data.</response>
    [HttpPost]
    public async Task<IActionResult> CreateAsync([FromForm] ProductCreateDTO createProductDTO)
    {
        var result = await _productService.CreateAsync(createProductDTO);
        return Created(
            $"/api/admin/products/{result}",
            new GenericResponse<string>("Product created successfully", result)
        );
    }

    /// <summary>
    /// Updates an existing product's information.
    /// </summary>
    /// <param name="id">The product identifier to update.</param>
    /// <param name="producUpdateDTO">Updated product data.</param>
    /// <returns>Updated product details.</returns>
    /// <response code="200">Product updated successfully.</response>
    /// <response code="404">Product not found.</response>
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateAsync(int id, [FromBody] ProducUpdateDTO producUpdateDTO)
    {
        var result = await _productService.UpdateAsync(id, producUpdateDTO);
        return Ok(new GenericResponse<ProductDetailDTO>("Product updated successfully", result));
    }

    /// <summary>
    /// Deactivates (soft deletes) a product.
    /// </summary>
    /// <param name="id">The product identifier to deactivate.</param>
    /// <returns>Success message.</returns>
    /// <response code="200">Product deactivated successfully.</response>
    /// <response code="404">Product not found.</response>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAsync(int id)
    {
        await _productService.ToggleActiveAsync(id);
        return Ok(new GenericResponse<string>("Product deactivated successfully"));
    }

    /// <summary>
    /// Uploads images for a product to Cloudinary.
    /// </summary>
    /// <param name="id">The product identifier.</param>
    /// <param name="images">List of image files to upload.</param>
    /// <returns>Success message.</returns>
    /// <response code="200">Images uploaded successfully.</response>
    /// <response code="400">No images provided.</response>
    [HttpPost("{id}/images")]
    public async Task<IActionResult> UploadImages(int id, [FromForm] List<IFormFile> images)
    {
        if (images == null || !images.Any())
        {
            return BadRequest(new GenericResponse<string>("No images provided."));
        }

        foreach (var image in images)
        {
            await _fileService.UploadAsync(image, id);
        }
        return Ok(new GenericResponse<string>("Images uploaded successfully"));
    }

    /// <summary>
    /// Deletes an image from a product.
    /// </summary>
    /// <param name="id">The product identifier.</param>
    /// <param name="imageId">The image identifier to delete.</param>
    /// <returns>Success message.</returns>
    /// <response code="200">Image deleted successfully.</response>
    /// <response code="404">Image not found.</response>
    [HttpDelete("{id}/images/{imageId}")]
    public async Task<IActionResult> DeleteImageAsync(int id, int imageId)
    {
        await _fileService.DeleteAsync(imageId);
        return Ok(new GenericResponse<string>("Image deleted successfully"));
    }

    /// <summary>
    /// Updates the discount percentage of a product.
    /// </summary>
    /// <param name="id">The product identifier.</param>
    /// <param name="dto">DTO containing the new discount percentage.</param>
    /// <returns>Success message.</returns>
    /// <response code="200">Product discount updated successfully.</response>
    /// <response code="400">Invalid discount value.</response>
    [HttpPatch("{id}/discount")]
    public async Task<IActionResult> UpdateDiscountAsync(
        int id,
        [FromBody] UpdateProductDiscountDTO dto
    )
    {
        await _productService.UpdateDiscountAsync(id, dto);
        return Ok(new GenericResponse<string>("Product discount updated successfully."));
    }

    /// <summary>
    /// Toggles the product status (active/inactive).
    /// </summary>
    /// <param name="id">The product identifier.</param>
    /// <returns>Success message.</returns>
    /// <response code="200">Product status updated successfully.</response>
    /// <response code="404">Product not found.</response>
    [HttpPatch("{id}/status")]
    public async Task<IActionResult> ToggleStatusAsync(int id)
    {
        await _productService.ToggleActiveAsync(id);
        return Ok(new GenericResponse<string>("Product status updated successfully"));
    }
}
