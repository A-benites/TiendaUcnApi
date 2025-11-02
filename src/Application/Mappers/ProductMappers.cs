using Mapster;
using TiendaUcnApi.src.Application.DTO.ProductDTO;
using TiendaUcnApi.src.Application.DTO.ProductDTO.AdminDTO;
using TiendaUcnApi.src.Application.DTO.ProductDTO.ConsumerDTO;
using TiendaUcnApi.Src.Application.DTO.ProductDTO.CustomerDTO;
using TiendaUcnApi.src.Domain.Models;

namespace TiendaUcnApi.src.Application.Mappers;

/// <summary>
/// Configures mappings between Product entities and Product DTOs using Mapster.
/// Handles transformations for different views (customer, admin) with pricing calculations and stock indicators.
/// </summary>
public class ProductMapper
{
    /// <summary>
    /// Application configuration (used to retrieve configuration values).
    /// </summary>
    private readonly IConfiguration _configuration;

    /// <summary>
    /// Default image URL for products without images.
    /// </summary>
    private readonly string? _defaultImageURL;

    /// <summary>
    /// Minimum quantity to consider as few units available.
    /// </summary>
    private readonly int _fewUnitsAvailable;

    /// <summary>
    /// Initializes a new instance of the <see cref="ProductMapper"/> class.
    /// </summary>
    /// <param name="configuration">Application configuration.</param>
    /// <exception cref="InvalidOperationException">Thrown when required configuration values are missing.</exception>
    public ProductMapper(IConfiguration configuration)
    {
        _configuration = configuration;
        _defaultImageURL =
            _configuration.GetValue<string>("Products:DefaultImageUrl")
            ?? throw new InvalidOperationException("The default image URL cannot be null.");
        _fewUnitsAvailable =
            _configuration.GetValue<int?>("Products:FewUnitsAvailable")
            ?? throw new InvalidOperationException(
                "The 'FewUnitsAvailable' configuration cannot be null."
            );
    }

    /// <summary>
    /// Configures all product-related mappings.
    /// </summary>
    public void ConfigureAllMappings()
    {
        ConfigureProductMappings();
    }

    /// <summary>
    /// Configures mappings between <see cref="Product"/> entities and their DTOs.
    /// Includes mappings for detailed view, customer view, admin view, and create operations.
    /// </summary>
    public void ConfigureProductMappings()
    {
        // Mapping for detailed product view
        TypeAdapterConfig<Product, ProductDetailDTO>
            .NewConfig()
            .Map(dest => dest.Id, src => src.Id)
            .Map(dest => dest.Title, src => src.Title)
            .Map(dest => dest.Description, src => src.Description)
            .Map(
                dest => dest.ImagesURL,
                src =>
                    src.Images.Count() != 0
                        ? src.Images.Select(i => i.ImageUrl).ToList()
                        : new List<string> { _defaultImageURL! }
            )
            .Map(dest => dest.Price, src => src.Price.ToString("C"))
            .Map(dest => dest.Discount, src => (int)src.Discount)
            .Map(
                dest => dest.FinalPrice,
                src => CalculateFinalPrice(src.Price, src.Discount).ToString("C")
            )
            .Map(dest => dest.Stock, src => src.Stock)
            .Map(dest => dest.StockIndicator, src => GetStockIndicator(src.Stock))
            .Map(dest => dest.CategoryName, src => src.Category.Name)
            .Map(dest => dest.BrandName, src => src.Brand.Name)
            .Map(dest => dest.StatusName, src => src.Status)
            .Map(dest => dest.IsAvailable, src => src.IsAvailable);

        // Mapping for customer product card view
        TypeAdapterConfig<Product, ProductForCustomerDTO>
            .NewConfig()
            .Map(dest => dest.Id, src => src.Id)
            .Map(dest => dest.Title, src => src.Title)
            .Map(dest => dest.Description, src => src.Description)
            .Map(
                dest => dest.MainImageURL,
                src =>
                    src.Images.FirstOrDefault() != null
                        ? src.Images.First().ImageUrl
                        : _defaultImageURL
            )
            .Map(dest => dest.Price, src => src.Price.ToString("C"))
            .Map(dest => dest.Discount, src => src.Discount);

        // Mapping for admin product list view
        TypeAdapterConfig<Product, ProductForAdminDTO>
            .NewConfig()
            .Map(dest => dest.Id, src => src.Id)
            .Map(dest => dest.Title, src => src.Title)
            .Map(
                dest => dest.MainImageURL,
                src =>
                    src.Images.FirstOrDefault() != null
                        ? src.Images.First().ImageUrl
                        : _defaultImageURL
            )
            .Map(dest => dest.Price, src => src.Price.ToString("C"))
            .Map(dest => dest.Stock, src => src.Stock)
            .Map(dest => dest.StockIndicator, src => GetStockIndicator(src.Stock))
            .Map(dest => dest.CategoryName, src => src.Category.Name)
            .Map(dest => dest.BrandName, src => src.Brand.Name)
            .Map(dest => dest.StatusName, src => src.Status)
            .Map(dest => dest.IsAvailable, src => src.IsAvailable);

        // Mapping for product creation
        TypeAdapterConfig<ProductCreateDTO, Product>
            .NewConfig()
            .Map(dest => dest.Title, src => src.Title)
            .Map(dest => dest.Description, src => src.Description)
            .Map(dest => dest.Price, src => src.Price)
            .Map(dest => dest.Stock, src => src.Stock)
            .Map(dest => dest.Status, src => src.Status);

        // Mapping for customer product list with FinalPrice calculation
        // Implements R71-R72 rubric requirement
        TypeAdapterConfig<Product, ListedProductsForCustomerDTO>
            .NewConfig()
            .Map(dest => dest.Id, src => src.Id)
            .Map(dest => dest.Title, src => src.Title)
            .Map(dest => dest.Description, src => src.Description)
            .Map(
                dest => dest.MainImageURL,
                src =>
                    src.Images.FirstOrDefault() != null
                        ? src.Images.First().ImageUrl
                        : _defaultImageURL
            )
            .Map(dest => dest.Price, src => src.Price)
            .Map(dest => dest.Discount, src => (int)src.Discount)
            .Map(dest => dest.FinalPrice, src => CalculateFinalPrice(src.Price, src.Discount))
            .Map(dest => dest.Stock, src => src.Stock)
            .Map(dest => dest.CategoryName, src => src.Category.Name)
            .Map(dest => dest.BrandName, src => src.Brand.Name);
    }

    /// <summary>
    /// Calculates the final price by applying the discount percentage.
    /// </summary>
    /// <param name="price">Original price.</param>
    /// <param name="discount">Discount percentage (0-100).</param>
    /// <returns>Final price with discount applied.</returns>
    private static decimal CalculateFinalPrice(decimal price, decimal discount)
    {
        return price * (1 - discount / 100);
    }

    /// <summary>
    /// Gets the stock indicator message based on available quantity.
    /// </summary>
    /// <param name="stock">Product stock quantity.</param>
    /// <returns>Stock indicator message: "Out of stock", "Few units available", or "In Stock".</returns>
    private string GetStockIndicator(int stock)
    {
        if (stock == 0)
        {
            return "Out of stock";
        }
        if (stock <= _fewUnitsAvailable)
        {
            return "Few units available";
        }
        return "In Stock"!;
    }
}
