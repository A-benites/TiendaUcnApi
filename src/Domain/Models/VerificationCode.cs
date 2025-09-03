using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TiendaUcnApi.src.Domain.Models;

public enum CodeType
{
    EmailVerification,
    PasswordReset,
    PasswordChange,
}

public class VerificationCode
{
    /// <summary>
    /// Identificador único del código de verificación.
    /// </summary>
    [Key]
    public int Id { get; set; }

    /// <summary>
    /// Tipo de código de verificación.
    /// </summary>
    // La columna en la base de datos no puede ser nula.
    [Required]
    public CodeType CodeType { get; set; }

    /// <summary>
    /// Código de verificación de 6 dígitos.
    /// </summary>
    [Required]
    [StringLength(6)]
    public string Code { get; set; }

    /// <summary>
    /// Cantidad de intentos fallidos realizados para usar el código.
    /// </summary>
    public int AttemptCount { get; set; } = 0;

    /// <summary>
    /// Fecha y hora de expiración del código de verificación.
    /// </summary>
    [Required]
    public DateTime ExpiryDate { get; set; }

    /// <summary>
    /// Fecha y hora en que se creó el código de verificación.
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // --- Relación con User ---

    /// <summary>
    /// Identificador único del usuario asociado al código de verificación.
    /// </summary>
    public int UserId { get; set; }

    /// <summary>
    /// Usuario asociado al código (propiedad de navegación).
    /// </summary>
    // Define la clave foránea para la relación con la tabla User.
    [ForeignKey("UserId")]
    public User User { get; set; } = null!;
}