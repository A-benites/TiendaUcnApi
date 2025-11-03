namespace TiendaUcnApi.src.Application.DTO.CartDTO
{
    /// <summary>
    /// Data Transfer Object representing an item in the shopping cart.
    /// </summary>
    public class CartItemDTO
    {
        /// <summary>
        /// Product identifier.
        /// </summary>
        public required int ProductId { get; set; }

        /// <summary>
        /// Product title/name.
        /// </summary>
        public required string ProductTitle { get; set; }

        /// <summary>
        /// URL of the product's main image.
        /// </summary>
        public required string ProductImageUrl { get; set; }

        /// <summary>
        /// Product price per unit.
        /// </summary>
        public required int Price { get; set; }

        /// <summary>
        /// Quantity of this product in the cart.
        /// </summary>
        public required int Quantity { get; set; }

        /// <summary>
        /// Discount applied to the product.
        /// </summary>
        public required int Discount { get; set; }

        /// <summary>
        /// Subtotal price for this item without discount (formatted as string).
        /// </summary>
        public required string SubTotalPrice { get; set; }

        /// <summary>
        /// Total price for this item including discount (formatted as string).
        /// </summary>
        public required string TotalPrice { get; set; }
    }
}
