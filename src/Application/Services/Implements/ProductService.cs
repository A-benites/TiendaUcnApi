using Mapster;
using Serilog;
using TiendaUcnApi.src.Application.DTO.ProductDTO;
using TiendaUcnApi.src.Application.DTO.ProductDTO.AdminDTO;
using TiendaUcnApi.src.Application.Services.Interfaces;
using TiendaUcnApi.src.Domain.Models;
using TiendaUcnApi.src.Infrastructure.Repositories.Interfaces;

namespace TiendaUcnApi.src.Application.Services.Implements;

/// <summary>
/// Servicio para la gestión de productos.
/// </summary>
public class ProductService : IProductService
{
    /// <summary>
    /// Repositorio de productos.
    /// </summary>
    private readonly IProductRepository _productRepository;

    /// <summary>
    /// Configuración de la aplicación.
    /// </summary>
    private readonly IConfiguration _configuration;

    /// <summary>
    /// Servicio para manejo de archivos (imágenes).
    /// </summary>
    private readonly IFileService _fileService;

    /// <summary>
    /// Tamaño de página por defecto para paginación.
    /// </summary>
    private readonly int _defaultPageSize;

    /// <summary>
    /// Constructor con todas las dependencias necesarias.
    /// </summary>
    /// <param name="productRepository">Repositorio de productos.</param>
    /// <param name="configuration">Configuración de la aplicación.</param>
    /// <param name="fileService">Servicio de archivos.</param>
    public ProductService(
        IProductRepository productRepository,
        IConfiguration configuration,
        IFileService fileService
    )
    {
        _productRepository = productRepository;
        _configuration = configuration;
        _fileService = fileService;
        _defaultPageSize = int.Parse(
            _configuration["Products:DefaultPageSize"]
                ?? throw new InvalidOperationException(
                    "La configuración 'DefaultPageSize' no está definida."
                )
        );
    }

    /// <summary>
    /// Crea un nuevo producto en el sistema.
    /// </summary>
    /// <param name="createProductDTO">Los datos del producto a crear.</param>
    /// <returns>ID del producto creado.</returns>
    public async Task<string> CreateAsync(ProductCreateDTO createProductDTO)
    {
        Product product = createProductDTO.Adapt<Product>();
        Category category =
            await _productRepository.GetCategoryByIdAsync(createProductDTO.CategoryId)
            ?? throw new Exception("La categoría especificada no existe.");
        Brand brand =
            await _productRepository.GetBrandByIdAsync(createProductDTO.BrandId)
            ?? throw new Exception("La marca especificada no existe.");
        product.CategoryId = category.Id;
        product.BrandId = brand.Id;
        product.Images = new List<Image>();
        int productId = await _productRepository.CreateAsync(product);
        Log.Information("Producto creado: {@Product}", product);
        if (createProductDTO.Images == null || !createProductDTO.Images.Any())
        {
            Log.Information("No se proporcionaron imágenes. Se asignará la imagen por defecto.");
            throw new InvalidOperationException(
                "Debe proporcionar al menos una imagen para el producto."
            );
        }
        foreach (var image in createProductDTO.Images)
        {
            Log.Information("Imagen asociada al producto: {@Image}", image);
            await _fileService.UploadAsync(image, productId);
        }
        return product.Id.ToString();
    }

    /// <summary>
    /// Retorna un producto específico por su ID.
    /// </summary>
    /// <param name="id">ID del producto.</param>
    /// <returns>DTO con los datos del producto.</returns>
    public async Task<ProductDetailDTO> GetByIdAsync(int id)
    {
        var product =
            await _productRepository.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Producto con ID {id} no encontrado.");
        Log.Information("Producto encontrado: {@Product}", product);
        return product.Adapt<ProductDetailDTO>();
    }

    /// <summary>
    /// Retorna un producto específico por su ID desde el punto de vista de un admin.
    /// </summary>
    /// <param name="id">ID del producto.</param>
    /// <returns>DTO con los datos del producto.</returns>
    public async Task<ProductDetailDTO> GetByIdForAdminAsync(int id)
    {
        var product =
            await _productRepository.GetByIdForAdminAsync(id)
            ?? throw new KeyNotFoundException($"Producto con ID {id} no encontrado.");
        Log.Information("Producto encontrado: {@Product}", product);
        return product.Adapt<ProductDetailDTO>();
    }

    /// <summary>
    /// Retorna todos los productos para el administrador según los parámetros de búsqueda.
    /// </summary>
    /// <param name="searchParams">Parámetros de búsqueda.</param>
    /// <returns>Lista paginada de productos para administración.</returns>
    public async Task<ListedProductsForAdminDTO> GetFilteredForAdminAsync(
        SearchParamsDTO searchParams
    )
    {
        Log.Information(
            "Obteniendo productos para administrador con parámetros de búsqueda: {@SearchParams}",
            searchParams
        );
        var (products, totalCount) = await _productRepository.GetFilteredForAdminAsync(
            searchParams
        );
        var totalPages = (int)
            Math.Ceiling((double)totalCount / (searchParams.PageSize ?? _defaultPageSize));
        int currentPage = searchParams.PageNumber;
        int pageSize = searchParams.PageSize ?? _defaultPageSize;
        if (currentPage < 1 || currentPage > totalPages)
        {
            throw new ArgumentOutOfRangeException("El número de página está fuera de rango.");
        }
        Log.Information(
            "Total de productos encontrados: {TotalCount}, Total de páginas: {TotalPages}, Página actual: {CurrentPage}, Tamaño de página: {PageSize}",
            totalCount,
            totalPages,
            currentPage,
            pageSize
        );

        return new ListedProductsForAdminDTO
        {
            Products = products.Adapt<List<ProductForAdminDTO>>(),
            TotalCount = totalCount,
            TotalPages = totalPages,
            CurrentPage = currentPage,
            PageSize = products.Count(),
        };
    }

    /// <summary>
    /// Cambia el estado activo de un producto por su ID.
    /// </summary>
    /// <param name="id">ID del producto.</param>
    public async Task ToggleActiveAsync(int id)
    {
        await _productRepository.ToggleActiveAsync(id);
    }

    /// <summary>
    /// Actualiza los datos de un producto.
    /// </summary>
    /// <param name="id">ID del producto.</param>
    /// <param name="producUpdateDTO">DTO con los datos a actualizar.</param>
    /// <returns>DTO con los datos actualizados del producto.</returns>
    public async Task<ProductDetailDTO> UpdateAsync(int id, ProducUpdateDTO producUpdateDTO)
    {
        var productToUpdate =
            await _productRepository.GetTrackedByIdForAdminAsync(id)
            ?? throw new KeyNotFoundException($"Producto con ID {id} no encontrado.");

        if (!string.IsNullOrEmpty(producUpdateDTO.Title))
        {
            productToUpdate.Title = producUpdateDTO.Title;
        }
        if (!string.IsNullOrEmpty(producUpdateDTO.Description))
        {
            productToUpdate.Description = producUpdateDTO.Description;
        }
        if (producUpdateDTO.Price.HasValue)
        {
            productToUpdate.Price = producUpdateDTO.Price.Value;
        }
        if (producUpdateDTO.Discount.HasValue)
        {
            productToUpdate.Discount = producUpdateDTO.Discount.Value;
        }
        if (producUpdateDTO.Stock.HasValue)
        {
            productToUpdate.Stock = producUpdateDTO.Stock.Value;
        }
        if (producUpdateDTO.Status.HasValue)
        {
            productToUpdate.Status = producUpdateDTO.Status.Value;
        }

        var updatedProduct = await _productRepository.UpdateAsync(productToUpdate);
        Log.Information("Producto actualizado: {@Product}", updatedProduct);

        return updatedProduct.Adapt<ProductDetailDTO>();
    }

    /// <summary>
    /// Actualiza el descuento de un producto.
    /// </summary>
    /// <param name="id">ID del producto.</param>
    /// <param name="dto">DTO con el nuevo descuento.</param>
    public async Task UpdateDiscountAsync(int id, UpdateProductDiscountDTO dto)
    {
        var product =
            await _productRepository.GetByIdForAdminAsync(id)
            ?? throw new KeyNotFoundException($"Producto con ID {id} no encontrado.");

        await _productRepository.UpdateDiscountAsync(id, dto.Discount);
        Log.Information(
            "Descuento del producto {ProductId} actualizado a {Discount}%",
            id,
            dto.Discount
        );
    }
}