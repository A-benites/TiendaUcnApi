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
    /// Unique Chilean user identifier (RUT).
    /// </summary>
    [Required]
    [RegularExpression(@"^(\d{1,2}\.\d{3}\.\d{3}-[\dkK])$", ErrorMessage = "RUT format is not valid.")]
    public required string Rut { get; set; }

    /// <summary>
    /// User's first name.
    /// </summary>
    [Required]
    [StringLength(50, MinimumLength = 2)]
    public required string FirstName { get; set; }

    /// <summary>
    /// User's last name.
    /// </summary>
    [Required]
    [StringLength(50, MinimumLength = 2)]
    public required string LastName { get; set; }

    /// <summary>
    /// User's gender.
    /// </summary>
    [Required]
    public Gender Gender { get; set; }

    /// <summary>
    /// User's birth date.
    /// </summary>
    [Required]
    public DateOnly BirthDate { get; set; }

    /// <summary>
    /// Verification codes associated with the user.
    /// </summary>
    public ICollection<VerificationCode> VerificationCodes { get; set; } = new List<VerificationCode>();

    /// <summary>
    /// Orders placed by the user.
    /// </summary>
    public ICollection<Order> Orders { get; set; } = new List<Order>();

    /// <summary>
    /// User registration date.
    /// </summary>
    public DateTime RegisteredAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Last profile update date.
    /// </summary>
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Indicates if the user was created by the seeder to differentiate from real users.
    /// </summary>
    public bool IsSeed { get; set; } = false;
}