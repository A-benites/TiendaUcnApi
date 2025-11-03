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
/// Service for managing products.
/// Handles product CRUD operations, image management, and filtering for both admin and customer views.
/// </summary>
public class ProductService : IProductService
{
    /// <summary>
    /// Product repository for data access.
    /// </summary>
    private readonly IProductRepository _productRepository;

    /// <summary>
    /// Application configuration.
    /// </summary>
    private readonly IConfiguration _configuration;

    /// <summary>
    /// File service for image upload/deletion operations.
    /// </summary>
    private readonly IFileService _fileService;

    /// <summary>
    /// Default page size for pagination.
    /// </summary>
    private readonly int _defaultPageSize;

    /// <summary>
    /// Initializes a new instance of the ProductService class.
    /// </summary>
    /// <param name="productRepository">Product repository.</param>
    /// <param name="configuration">Application configuration.</param>
    /// <param name="fileService">File service for image management.</param>
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
                    "The 'DefaultPageSize' configuration is not defined."
                )
        );
    }

    /// <summary>
    /// Creates a new product with associated images.
    /// Validates category and brand existence, requires at least one image.
    /// </summary>
    /// <param name="createProductDTO">DTO containing product creation data.</param>
    /// <returns>ID of the created product as a string.</returns>
    /// <exception cref="Exception">Thrown when category or brand doesn't exist.</exception>
    /// <exception cref="InvalidOperationException">Thrown when no images are provided.</exception>
    public async Task<string> CreateAsync(ProductCreateDTO createProductDTO)
    {
        Product product = createProductDTO.Adapt<Product>();
        Category category =
            await _productRepository.GetCategoryByIdAsync(createProductDTO.CategoryId)
            ?? throw new Exception("The specified category does not exist.");
        Brand brand =
            await _productRepository.GetBrandByIdAsync(createProductDTO.BrandId)
            ?? throw new Exception("The specified brand does not exist.");
        product.CategoryId = category.Id;
        product.BrandId = brand.Id;
        product.Images = new List<Image>();
        int productId = await _productRepository.CreateAsync(product);
        Log.Information("Product created: {@Product}", product);
        if (createProductDTO.Images == null || !createProductDTO.Images.Any())
        {
            Log.Information("No images provided. Default image will be assigned.");
            throw new InvalidOperationException(
                "You must provide at least one image for the product."
            );
        }
        foreach (var image in createProductDTO.Images)
        {
            Log.Information("Image associated with product: {@Image}", image);
            await _fileService.UploadAsync(image, productId);
        }
        return product.Id.ToString();
    }

    /// <summary>
    /// Retrieves a product by ID (general access).
    /// </summary>
    /// <param name="id">Product ID.</param>
    /// <returns>Product detail DTO.</returns>
    /// <exception cref="KeyNotFoundException">Thrown when product is not found.</exception>
    public async Task<ProductDetailDTO> GetByIdAsync(int id)
    {
        var product =
            await _productRepository.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Product with ID {id} not found.");
        Log.Information("Product found: {@Product}", product);
        return product.Adapt<ProductDetailDTO>();
    }

    /// <summary>
    /// Retrieves a product by ID for administrators.
    /// Includes all product details regardless of availability status.
    /// </summary>
    /// <param name="id">Product ID.</param>
    /// <returns>Product detail DTO.</returns>
    /// <exception cref="KeyNotFoundException">Thrown when product is not found.</exception>
    public async Task<ProductDetailForAdminDTO> GetByIdForAdminAsync(int id)
    {
        var product =
            await _productRepository.GetByIdForAdminAsync(id)
            ?? throw new KeyNotFoundException($"Product with ID {id} not found.");
        Log.Information("Product found: {@Product}", product);
        return product.Adapt<ProductDetailForAdminDTO>();
    }

    /// <summary>
    /// Retrieves a product by ID for customers.
    /// Only returns products that are available for purchase.
    /// </summary>
    /// <param name="id">Product ID.</param>
    /// <returns>Generic response containing product detail DTO.</returns>
    /// <exception cref="KeyNotFoundException">Thrown when product is not found or not available.</exception>
    public async Task<GenericResponse<ProductDetailDTO>> GetByIdForCustomerAsync(int id)
    {
        var product = await _productRepository.GetByIdForCustomerAsync(id);
        if (product == null)
        {
            throw new KeyNotFoundException($"Product with ID {id} not found.");
        }

        return new GenericResponse<ProductDetailDTO>(
            "Product found",
            product.Adapt<ProductDetailDTO>()
        );
    }

    /// <summary>
    /// Retrieves filtered and paginated products for administrators.
    /// Includes all products regardless of availability with search and pagination.
    /// </summary>
    /// <param name="searchParams">Search parameters including search term and pagination.</param>
    /// <returns>Listed products DTO with pagination metadata.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when page number is out of range.</exception>
    public async Task<ListedProductsForAdminDTO> GetFilteredForAdminAsync(
        SearchParamsDTO searchParams
    )
    {
        Log.Information(
            "Retrieving products for admin with search parameters: {@SearchParams}",
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
            throw new ArgumentOutOfRangeException("The page number is out of range.");
        }
        Log.Information(
            "Total products found: {TotalCount}, Total pages: {TotalPages}, Current page: {CurrentPage}, Page size: {PageSize}",
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
    /// Retrieves filtered and paginated products for customers.
    /// Implements R67-R70: Supports filtering by category, brand, price range, status, and sorting.
    /// Only returns available products.
    /// </summary>
    /// <param name="searchParams">Search parameters including filters, sorting, and pagination.</param>
    /// <returns>Generic response containing product list with complete pagination metadata.</returns>
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

        return new GenericResponse<object>("Products found", response);
    }

    /// <summary>
    /// Toggles the active status of a product.
    /// Switches IsAvailable between true and false.
    /// </summary>
    /// <param name="id">Product ID.</param>
    public async Task ToggleActiveAsync(int id)
    {
        await _productRepository.ToggleActiveAsync(id);
    }

    /// <summary>
    /// Updates product information.
    /// Only updates fields that are provided in the DTO (non-null values).
    /// </summary>
    /// <param name="id">Product ID to update.</param>
    /// <param name="producUpdateDTO">DTO containing updated product data.</param>
    /// <returns>Updated product detail DTO.</returns>
    /// <exception cref="KeyNotFoundException">Thrown when product is not found.</exception>
    public async Task<ProductDetailDTO> UpdateAsync(int id, ProducUpdateDTO producUpdateDTO)
    {
        var productToUpdate =
            await _productRepository.GetTrackedByIdForAdminAsync(id)
            ?? throw new KeyNotFoundException($"Product with ID {id} not found.");

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
        Log.Information("Product updated: {@Product}", updatedProduct);

        return updatedProduct.Adapt<ProductDetailDTO>();
    }

    /// <summary>
    /// Updates the discount percentage for a product.
    /// </summary>
    /// <param name="id">Product ID.</param>
    /// <param name="dto">DTO containing the new discount percentage.</param>
    /// <exception cref="KeyNotFoundException">Thrown when product is not found.</exception>
    public async Task UpdateDiscountAsync(int id, UpdateProductDiscountDTO dto)
    {
        var product =
            await _productRepository.GetByIdForAdminAsync(id)
            ?? throw new KeyNotFoundException($"Product with ID {id} not found.");

        await _productRepository.UpdateDiscountAsync(id, dto.Discount);
        Log.Information(
            "Discount for product {ProductId} updated to {Discount}%",
            id,
            dto.Discount
        );
    }
}
