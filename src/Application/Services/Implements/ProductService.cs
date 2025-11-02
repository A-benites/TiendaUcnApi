using Mapster;
using Serilog;
using TiendaUcnApi.src.Application.DTO;
using TiendaUcnApi.src.Application.DTO.BaseResponse;
using TiendaUcnApi.src.Application.DTO.ProductDTO;
using TiendaUcnApi.src.Application.DTO.ProductDTO.AdminDTO;
using TiendaUcnApi.src.Application.DTO.ProductDTO.ConsumerDTO;
using TiendaUcnApi.src.Application.Services.Interfaces;
using TiendaUcnApi.src.Domain.Models;
using TiendaUcnApi.src.Infrastructure.Repositories.Interfaces;

namespace TiendaUcnApi.src.Application.Services.Implements;

/// <summary>
/// Servicio para la gestión de productos.
/// </summary>
public class ProductService : IProductService
{
    private readonly IProductRepository _productRepository;
    private readonly IConfiguration _configuration;
    private readonly IFileService _fileService;
    private readonly int _defaultPageSize;

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

    public async Task<ProductDetailDTO> GetByIdAsync(int id)
    {
        var product =
            await _productRepository.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Producto con ID {id} no encontrado.");
        Log.Information("Producto encontrado: {@Product}", product);
        return product.Adapt<ProductDetailDTO>();
    }

    public async Task<ProductDetailDTO> GetByIdForAdminAsync(int id)
    {
        var product =
            await _productRepository.GetByIdForAdminAsync(id)
            ?? throw new KeyNotFoundException($"Producto con ID {id} no encontrado.");
        Log.Information("Producto encontrado: {@Product}", product);
        return product.Adapt<ProductDetailDTO>();
    }

    public async Task<GenericResponse<ProductDetailDTO>> GetByIdForCustomerAsync(int id)
    {
        var product = await _productRepository.GetByIdForCustomerAsync(id);
        if (product == null)
        {
            throw new KeyNotFoundException($"Producto con ID {id} no encontrado.");
        }

        return new GenericResponse<ProductDetailDTO>(
            "Producto encontrado",
            product.Adapt<ProductDetailDTO>()
        );
    }

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

    public async Task<GenericResponse<object>> GetFilteredForCustomerAsync(
        SearchParamsDTO searchParams
    )
    {
        var (products, totalCount) = await _productRepository.GetFilteredForCustomerAsync(
            searchParams
        );
        var productsDto = products.Adapt<List<ListedProductsForCustomerDTO>>();

        // R67: Include complete pagination metadata
        var pageSize = searchParams.PageSize ?? 10; // Default page size
        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

        var response = new ProductListResponseDTO
        {
            Products = productsDto,
            TotalCount = totalCount,
            Page = searchParams.PageNumber,
            PageSize = pageSize,
            TotalPages = totalPages,
        };

        return new GenericResponse<object>("Productos encontrados", response);
    }

    public async Task ToggleActiveAsync(int id)
    {
        await _productRepository.ToggleActiveAsync(id);
    }

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
