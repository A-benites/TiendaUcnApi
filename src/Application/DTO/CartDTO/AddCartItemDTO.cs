using System.ComponentModel.DataAnnotations;

namespace TiendaUcnApi.src.Application.DTO.CartDTO
{
    /// <summary>
    /// Data Transfer Object for adding an item to the shopping cart.
    /// </summary>
    public class AddCartItemDTO
    {
        /// <summary>
        /// Product identifier. Must be a positive integer.
        /// </summary>
        [Required(ErrorMessage = "El ID del producto es requerido.")]
        [Range(1, int.MaxValue, ErrorMessage = "El ID del producto debe ser un número positivo.")]
        public int ProductId { get; set; }

        /// <summary>
        /// Quantity to add. Must be a positive integer.
        /// </summary>
        [Required(ErrorMessage = "La cantidad es requerida.")]
        [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser un número positivo.")]
        public int Quantity { get; set; }
    }
}
