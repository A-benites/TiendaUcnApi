using System.ComponentModel.DataAnnotations;

namespace TiendaUcnApi.src.Application.DTO.CartDTO
{
    /// <summary>
    /// Data Transfer Object for changing the quantity of an item in the cart.
    /// </summary>
    public class ChangeItemQuantityDTO
    {
        /// <summary>
        /// Product identifier. Must be a positive integer.
        /// </summary>
        [Required(ErrorMessage = "El ID del producto es requerido.")]
        [Range(1, int.MaxValue, ErrorMessage = "El ID del producto debe ser un número positivo.")]
        public required int ProductId { get; set; }

        /// <summary>
        /// New quantity for the item. Must be a positive integer.
        /// </summary>
        [Required(ErrorMessage = "La cantidad es requerida.")]
        [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser un número positivo.")]
        public required int Quantity { get; set; }
    }
}
