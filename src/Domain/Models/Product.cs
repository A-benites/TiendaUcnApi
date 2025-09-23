using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TiendaUcnApi.src.Domain.Models;

public enum Status
{
    New,
    Used,
}

public class Product
{
    /// <summary>
    /// Unique identifier for the product.
    /// </summary>
    [Key]
    public int Id { get; set; }

    /// <summary>
    /// Product title.
    /// </summary>
    [Required]
    [StringLength(50, MinimumLength = 3)]
    public required string Title { get; set; }

    /// <summary>
    /// Product description.
    /// </summary>
    [Required]
    [MinLength(20, ErrorMessage = "Description must be at least 20 characters.")]
    public required string Description { get; set; }

    /// <summary>
    /// Product price.
    /// </summary>
    [Required]
    [Range(0.01, (double)decimal.MaxValue, ErrorMessage = "Price must be a positive value.")]
    public decimal Price { get; set; }

    /// <summary>
    /// Product discount.
    /// </summary>
    [Range(0, (double)decimal.MaxValue, ErrorMessage = "Discount cannot be negative.")]
    public decimal Discount { get; set; }

    /// <summary>
    /// Product stock.
    /// </summary>
    [Required]
    [Range(0, int.MaxValue)]
    public int Stock { get; set; }

    /// <summary>
    /// Product status (New or Used).
    /// </summary>
    [Required]
    public Status Status { get; set; }

    /// <summary>
    /// Indicates if the product is logically deleted (not shown to the customer).
    /// </summary>
    public bool IsAvailable { get; set; } = true;

    /// <summary>
    /// Product creation date.
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Product update date.
    /// </summary>
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // --- Relationships with other tables ---

    /// <summary>
    /// Category identifier for the product.
    /// </summary>
    public int CategoryId { get; set; }

    /// <summary>
    /// Product category (navigation property).
    /// </summary>
    [ForeignKey("CategoryId")]
    public Category Category { get; set; } = null!;

    /// <summary>
    /// Brand identifier for the product.
    /// </summary>
    public int BrandId { get; set; }

    /// <summary>
    /// Product brand (navigation property).
    /// </summary>
    [ForeignKey("BrandId")]
    public Brand Brand { get; set; } = null!;

    /// <summary>
    /// List of images associated with the product.
    /// </summary>
    public ICollection<Image> Images { get; set; } = new List<Image>();
}