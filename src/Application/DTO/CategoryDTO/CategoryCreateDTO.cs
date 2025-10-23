using System.ComponentModel.DataAnnotations;

namespace TiendaUcnApi.src.Application.DTO.CategoryDTO
{
    /// <summary>
    /// DTO para la creación de una nueva categoría.
    /// </summary>
    public class CategoryCreateDTO
    {
        /// <summary>
        /// Nombre de la categoría.
        /// </summary>
        [Required(ErrorMessage = "El nombre de la categoría es obligatorio.")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "El nombre debe tener entre 2 y 50 caracteres.")]
        public required string Name { get; set; }
    }
}
