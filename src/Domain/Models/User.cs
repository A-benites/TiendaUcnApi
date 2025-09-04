using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace TiendaUcnApi.src.Domain.Models;

public enum Gender
{
    Masculino,
    Femenino,
    Otro,
}

public class User : IdentityUser<int>
{
    /// <summary>
    /// Identificador único del usuario chileno.
    /// </summary>
    // La columna en la base de datos no puede ser nula.
    [Required]
    // Define el largo máximo de la columna en la base de datos.
    [StringLength(12)]
    public string Rut { get; set; }

    /// <summary>
    /// Nombre del usuario.
    /// </summary>
    // La columna en la base de datos no puede ser nula.
    [Required]
    // Define el largo máximo y mínimo de la columna.
    [StringLength(50, MinimumLength = 2)]
    public string FirstName { get; set; }

    /// <summary>
    /// Apellido del usuario.
    /// </summary>
    // La columna en la base de datos no puede ser nula.
    [Required]
    // Define el largo máximo y mínimo de la columna.
    [StringLength(50, MinimumLength = 2)]
    public string LastName { get; set; }

    /// <summary>
    /// Género del usuario.
    /// </summary>
    // La columna en la base de datos no puede ser nula.
    [Required]
    public Gender Gender { get; set; }

    /// <summary>
    /// Fecha de nacimiento del usuario.
    /// </summary>
    // La columna en la base de datos no puede ser nula.
    [Required]
    public DateTime BirthDate { get; set; }

    /// <summary>
    /// Códigos de verificación asociados al usuario.
    /// </summary>
    public ICollection<VerificationCode> VerificationCodes { get; set; } = new List<VerificationCode>();

    /// <summary>
    /// Órdenes realizadas por el usuario.
    /// </summary>
    public ICollection<Order> Orders { get; set; } = new List<Order>();

    /// <summary>
    /// Fecha de registro del usuario.
    /// </summary>
    public DateTime RegisteredAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Fecha de la última actualización del perfil del usuario.
    /// </summary>
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Indica si el usuario fue creado por el seeder para diferenciarlo de usuarios reales.
    /// </summary>
    public bool IsSeed { get; set; } = false;
}