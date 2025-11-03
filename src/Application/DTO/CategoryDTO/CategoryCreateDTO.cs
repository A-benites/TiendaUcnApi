using System.ComponentModel.DataAnnotations;

namespace TiendaUcnApi.src.Application.DTO.CategoryDTO
{
    /// <summary>
    /// Data Transfer Object for creating a new category.
    /// </summary>
    public class CategoryCreateDTO
    {
        /// <summary>
        /// Category name. Must be between 2 and 50 characters.
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
