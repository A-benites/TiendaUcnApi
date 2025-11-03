namespace TiendaUcnApi.src.Application.DTO.CartDTO
{
    /// <summary>
    /// Data Transfer Object representing a shopping cart.
    /// </summary>
    public class CartDTO
    {
        /// <summary>
        /// Unique identifier for anonymous/unauthenticated users' carts.
        /// </summary>
        public required string BuyerId { get; set; }

        /// <summary>
        /// User ID if the cart belongs to an authenticated user, null otherwise.
        /// </summary>
        public required int? UserId { get; set; }

        /// <summary>
        /// List of items in the shopping cart.
        /// </summary>
        public required List<CartItemDTO> Items { get; set; } = new List<CartItemDTO>();

        /// <summary>
        /// Subtotal price without discounts (formatted as string).
        /// </summary>
        public required string SubTotalPrice { get; set; }

        /// <summary>
        /// Total price including discounts (formatted as string).
        /// </summary>
        public required string TotalPrice { get; set; }

        /// <summary>
        /// List of product IDs that were removed because they became unavailable.
        /// Implements R49 rubric requirement to report excluded products.
        /// </summary>
        public List<int>? RemovedUnavailableProducts { get; set; }
    }
}
