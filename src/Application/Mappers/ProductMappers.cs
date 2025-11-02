using Mapster;
using TiendaUcnApi.src.Application.DTO.ProductDTO;
using TiendaUcnApi.src.Application.DTO.ProductDTO.AdminDTO;
using TiendaUcnApi.src.Application.DTO.ProductDTO.ConsumerDTO;
using TiendaUcnApi.Src.Application.DTO.ProductDTO.CustomerDTO;
using TiendaUcnApi.src.Domain.Models;

namespace TiendaUcnApi.src.Application.Mappers;

/// <summary>
/// Clase encargada de configurar los mapeos entre entidades de producto y sus DTOs.
/// </summary>
public class ProductMapper
{
    /// <summary>
    /// Configuración de la aplicación (usada para obtener valores de configuración).
    /// </summary>
    private readonly IConfiguration _configuration;

    /// <summary>
    /// URL de la imagen por defecto para productos sin imágenes.
    /// </summary>
    private readonly string? _defaultImageURL;

    /// <summary>
    /// Cantidad mínima para considerar que hay pocas unidades disponibles.
    /// </summary>
    private readonly int _fewUnitsAvailable;

    /// <summary>
    /// Inicializa una nueva instancia de <see cref="ProductMapper"/>.
    /// </summary>
    /// <param name="configuration">Configuración de la aplicación.</param>
    public ProductMapper(IConfiguration configuration)
    {
        _configuration = configuration;
        _defaultImageURL =
            _configuration.GetValue<string>("Products:DefaultImageUrl")
            ?? throw new InvalidOperationException(
                "La URL de la imagen por defecto no puede ser nula."
            );
        _fewUnitsAvailable =
            _configuration.GetValue<int?>("Products:FewUnitsAvailable")
            ?? throw new InvalidOperationException(
                "La configuración 'FewUnitsAvailable' no puede ser nula."
            );
    }

    /// <summary>
    /// Configura todos los mapeos relacionados con productos.
    /// </summary>
    public void ConfigureAllMappings()
    {
        ConfigureProductMappings();
    }

    /// <summary>
    /// Configura los mapeos entre entidades <see cref="Product"/> y sus DTOs.
    /// </summary>
    public void ConfigureProductMappings()
    {
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
    /// Calcula el precio final aplicando el descuento.
    /// </summary>
    /// <param name="price">Precio original.</param>
    /// <param name="discount">Porcentaje de descuento.</param>
    /// <returns>Precio final con descuento aplicado.</returns>
    private static decimal CalculateFinalPrice(decimal price, decimal discount)
    {
        return price * (1 - discount / 100);
    }

    /// <summary>
    /// Obtiene el indicador de stock basado en la cantidad disponible.
    /// </summary>
    /// <param name="stock">Stock del producto.</param>
    /// <returns>Retorna el mensaje adecuado según el stock.</returns>
    private string GetStockIndicator(int stock)
    {
        if (stock == 0)
        {
            return "Producto sin stock";
        }
        if (stock <= _fewUnitsAvailable)
        {
            return "Pocas unidades disponibles";
        }
        return "Con Stock"!;
    }
}
