using Mapster;
using TiendaUcnApi.src.Application.DTO.CartDTO;
using TiendaUcnApi.src.Domain.Models;

namespace Tienda_UCN_api.Src.Application.Mappers
{
    /// <summary>
    /// Configures mappings between Cart entities and Cart DTOs using Mapster.
    /// Handles transformations for shopping cart display with pricing calculations.
    /// </summary>
    public class CartMapper
    {
        private readonly IConfiguration _configuration;
        private readonly string? _defaultImageURL;

        /// <summary>
        /// Initializes a new instance of the <see cref="CartMapper"/> class.
        /// </summary>
        /// <param name="configuration">Application configuration to retrieve default image URL.</param>
        /// <exception cref="InvalidOperationException">Thrown when default image URL is not configured.</exception>
        public CartMapper(IConfiguration configuration)
        {
            _configuration = configuration;
            _defaultImageURL =
                _configuration.GetValue<string>("Products:DefaultImageUrl")
                ?? throw new InvalidOperationException("The default image URL cannot be null.");
        }

        /// <summary>
        /// Configures all cart-related mappings.
        /// </summary>
        public void ConfigureAllMappings()
        {
            ConfigureCartItemMappings();
            ConfigureCartMappings();
        }

        /// <summary>
        /// Configures the mapping from Cart entity to CartDTO.
        /// Maps buyer information, cart items, and calculates total pricing.
        /// </summary>
        public void ConfigureCartMappings()
        {
            TypeAdapterConfig<Cart, CartDTO>
                .NewConfig()
                .Map(dest => dest.BuyerId, src => src.BuyerId)
                .Map(dest => dest.UserId, src => src.UserId)
                .Map(dest => dest.SubTotalPrice, src => src.SubTotal)
                .Map(dest => dest.Items, src => src.CartItems)
                .Map(dest => dest.TotalPrice, src => src.Total);
        }

        /// <summary>
        /// Configures the mapping from CartItem entity to CartItemDTO.
        /// Maps product information, images, quantities, and calculates item pricing with discounts.
        /// </summary>
        public void ConfigureCartItemMappings()
        {
            TypeAdapterConfig<CartItem, CartItemDTO>
                .NewConfig()
                .Map(dest => dest.ProductId, src => src.ProductId)
                .Map(dest => dest.ProductTitle, src => src.Product.Title)
                .Map(
                    dest => dest.ProductImageUrl,
                    src =>
                        src.Product.Images != null && src.Product.Images.Any()
                            ? src.Product.Images.First().ImageUrl
                            : _defaultImageURL
                )
                .Map(dest => dest.Price, src => src.Product.Price)
                .Map(dest => dest.Discount, src => src.Product.Discount)
                .Map(dest => dest.Quantity, src => src.Quantity)
                .Map(
                    dest => dest.SubTotalPrice,
                    src => (src.Product.Price * src.Quantity).ToString("C")
                )
                .Map(
                    dest => dest.TotalPrice,
                    src =>
                        (
                            src.Product.Price
                            * src.Quantity
                            * (1 - (decimal)src.Product.Discount / 100)
                        ).ToString("C")
                );
        }
    }
}
