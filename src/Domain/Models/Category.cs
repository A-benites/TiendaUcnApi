using System.ComponentModel.DataAnnotations;

namespace TiendaUcnApi.src.Domain.Models;

public class Category
{
    /// <summary>
    /// Unique identifier for the category.
    /// </summary>
    [Key]
    public int Id { get; set; }

    /// <summary>
    /// Category name.
    /// </summary>
    [Required]
    [StringLength(50)]
    public string Name { get; set; }

    /// <summary>
    /// Category creation date.
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Collection of products belonging to this category.
    /// </summary>
    public ICollection<Product> Products { get; set; } = new List<Product>();
}