using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TiendaUcnApi.src.Domain.Models;

public class Image
{
    /// <summary>
    /// Unique identifier for the image.
    /// </summary>
    [Key]
    public int Id { get; set; }

    /// <summary>
    /// Public URL of the image hosted on an external service (e.g., Cloudinary).
    /// </summary>
    [Required]
    [Url(ErrorMessage = "The image URL format is not valid.")]
    public string ImageUrl { get; set; }

    /// <summary>
    /// Public identifier used by the external service to manage the image.
    /// </summary>
    [Required]
    public string PublicId { get; set; }

    /// <summary>
    /// Image record creation date.
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // --- Relationship with Product ---

    /// <summary>
    /// Identifier of the product to which this image belongs.
    /// </summary>
    public int ProductId { get; set; }

    /// <summary>
    /// Product associated with the image (navigation property).
    /// </summary>
    [ForeignKey("ProductId")]
    public Product Product { get; set; } = null!;
}