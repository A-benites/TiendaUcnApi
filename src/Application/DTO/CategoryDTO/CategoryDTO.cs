namespace TiendaUcnApi.src.Application.DTO.CategoryDTO
{
    /// <summary>
    /// DTO para representar una categoría existente en la API.
    /// </summary>
    public class CategoryDTO
    {
        /// <summary>
        /// Identificador único de la categoría.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Nombre de la categoría.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Fecha de creación de la categoría.
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Número total de productos asociados a la categoría.
        /// </summary>
        public int ProductCount { get; set; }
    }
}
