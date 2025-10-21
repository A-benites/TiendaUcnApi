
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TiendaUcnApi.src.Application.DTO;
using TiendaUcnApi.src.Application.DTO.ProductDTO;
using TiendaUcnApi.src.Application.DTO.ProductDTO.AdminDTO;
using TiendaUcnApi.src.Application.Services.Interfaces;

namespace TiendaUcnApi.src.API.Controllers;

/// <summary>
/// Controlador para la administración de productos.
/// Permite CRUD, gestión de imágenes y descuentos.
/// Solo accesible por usuarios con rol "Administrador".
/// </summary>
[ApiController]
[Route("api/admin/products")] // Ruta base ajustada para todos los endpoints de admin
[Authorize(Roles = "Administrador")] // Protección a nivel de controlador
public class AdminProductController : ControllerBase // Cambiado para evitar conflicto de rutas
{
    private readonly IProductService _productService;
    private readonly IFileService _fileService;

    public AdminProductController(IProductService productService, IFileService fileService)
    {
        _productService = productService;
        _fileService = fileService;
    }

    /// <summary>
    /// Obtiene todos los productos para administración, con filtros.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAllForAdminAsync([FromQuery] SearchParamsDTO searchParams)
    {
        var result = await _productService.GetFilteredForAdminAsync(searchParams);
        return Ok(
            new GenericResponse<ListedProductsForAdminDTO>(
                "Productos obtenidos exitosamente",
                result
            )
        );
    }

    /// <summary>
    /// Obtiene el detalle de un producto por ID para administración.
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetByIdForAdminAsync(int id)
    {
        var result = await _productService.GetByIdForAdminAsync(id);
        return Ok(new GenericResponse<ProductDetailDTO>("Producto obtenido exitosamente", result));
    }

    /// <summary>
    /// Crea un nuevo producto.
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> CreateAsync([FromForm] ProductCreateDTO createProductDTO)
    {
        var result = await _productService.CreateAsync(createProductDTO);
        return Created(
            $"/api/admin/products/{result}",
            new GenericResponse<string>("Producto creado exitosamente", result)
        );
    }

    /// <summary>
    /// Actualiza un producto existente.
    /// </summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateAsync(int id, [FromBody] ProducUpdateDTO producUpdateDTO)
    {
        var result = await _productService.UpdateAsync(id, producUpdateDTO);
        return Ok(
            new GenericResponse<ProductDetailDTO>("Producto actualizado exitosamente", result)
        );
    }

    /// <summary>
    /// Desactiva (elimina lógicamente) un producto.
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAsync(int id)
    {
        await _productService.ToggleActiveAsync(id);
        return Ok(new GenericResponse<string>("Producto desactivado exitosamente"));
    }

    /// <summary>
    /// Sube imágenes para un producto.
    /// </summary>
    [HttpPost("{id}/images")]
    public async Task<IActionResult> UploadImages(int id, [FromForm] List<IFormFile> images)
    {
        if (images == null || !images.Any())
        {
            return BadRequest(new GenericResponse<string>("No se proporcionaron imágenes."));
        }

        foreach (var image in images)
        {
            await _fileService.UploadAsync(image, id);
        }
        return Ok(new GenericResponse<string>("Imágenes subidas exitosamente"));
    }

    /// <summary>
    /// Elimina una imagen de un producto.
    /// </summary>
    [HttpDelete("{id}/images/{imageId}")]
    public async Task<IActionResult> DeleteImageAsync(int id, int imageId)
    {
        await _fileService.DeleteAsync(imageId);
        return Ok(new GenericResponse<string>("Imagen eliminada exitosamente"));
    }

    /// <summary>
    /// Actualiza el descuento de un producto.
    /// </summary>
    [HttpPatch("{id}/discount")]
    public async Task<IActionResult> UpdateDiscountAsync(
        int id,
        [FromBody] UpdateProductDiscountDTO dto
    )
    {
        await _productService.UpdateDiscountAsync(id, dto);
        return Ok(new GenericResponse<string>("Descuento del producto actualizado exitosamente."));
    }

    /// <summary>
    /// Cambia el estado (activo/inactivo) de un producto.
    /// </summary>
    [HttpPatch("{id}/status")]
    public async Task<IActionResult> ToggleStatusAsync(int id)
    {
        await _productService.ToggleActiveAsync(id);
        return Ok(new GenericResponse<string>("Estado del producto actualizado exitosamente"));
    }
}
