using System.ComponentModel.DataAnnotations;

namespace TiendaUcnApi.src.Domain.Models;

public class Brand
{
    /// <summary>
    /// Unique identifier for the brand.
    /// </summary>
    [Key]
    public int Id { get; set; }

    /// <summary>
    /// Brand name.
    /// </summary>
    [Required]
    [StringLength(50)]
    public required string Name { get; set; }

    /// <summary>
    /// Brand creation date.
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Collection of products belonging to this brand.
    /// </summary>
    public ICollection<Product> Products { get; set; } = new List<Product>();
}