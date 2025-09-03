using System.ComponentModel.DataAnnotations;

namespace TiendaUcnApi.src.Domain.Models;

public class Category
{
    /// <summary>
    /// Identificador único de la categoría.
    /// </summary>
    [Key]
    public int Id { get; set; }

    /// <summary>
    /// Nombre de la categoría.
    /// </summary>
    // La columna no puede ser nula.
    [Required]
    // Define el largo máximo de la columna en la base de datos.
    [StringLength(50)]
    public string Name { get; set; }

    /// <summary>
    /// Fecha de creación de la categoría.
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Colección de productos que pertenecen a esta categoría.
    /// </summary>
    // Propiedad de navegación para la relación uno-a-muchos con Product.
    public ICollection<Product> Products { get; set; } = new List<Product>();
}