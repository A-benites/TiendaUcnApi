namespace TiendaUcnApi.src.Application.DTO.CategoryDTO
{
    /// <summary>
    /// Data Transfer Object representing an existing category in the API.
    /// </summary>
    public class CategoryDTO
    {
        /// <summary>
        /// Unique identifier for the category.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Category name.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Date when the category was created.
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Total number of products associated with this category.
        /// </summary>
        public int ProductCount { get; set; }
    }
}
