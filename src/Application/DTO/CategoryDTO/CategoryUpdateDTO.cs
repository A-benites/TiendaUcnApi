using System.ComponentModel.DataAnnotations;

namespace TiendaUcnApi.src.Application.DTO.CategoryDTO
{
    /// <summary>
    /// Data Transfer Object for updating an existing category.
    /// </summary>
    public class CategoryUpdateDTO
    {
        /// <summary>
        /// New category name. Must be between 2 and 50 characters.
        /// </summary>
        [Required(ErrorMessage = "El nombre de la categoría es obligatorio.")]
        [StringLength(
            50,
            MinimumLength = 2,
            ErrorMessage = "El nombre debe tener entre 2 y 50 caracteres."
        )]
        public required string Name { get; set; }
    }
}
