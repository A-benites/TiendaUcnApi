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
    /// Unique identifier for the verification code.
    /// </summary>
    [Key]
    public int Id { get; set; }

    /// <summary>
    /// Type of verification code.
    /// </summary>
    [Required]
    public CodeType CodeType { get; set; }

    /// <summary>
    /// 6-digit verification code.
    /// </summary>
    [Required]
    [StringLength(6)]
    public string Code { get; set; }

    /// <summary>
    /// Number of failed attempts to use the code.
    /// </summary>
    public int AttemptCount { get; set; } = 0;

    /// <summary>
    /// Expiry date and time of the verification code.
    /// </summary>
    [Required]
    public DateTime ExpiryDate { get; set; }

    /// <summary>
    /// Creation date and time of the verification code.
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // --- Relationship with User ---

    /// <summary>
    /// Unique identifier of the user associated with the verification code.
    /// </summary>
    public int UserId { get; set; }

    /// <summary>
    /// User associated with the code (navigation property).
    /// </summary>
    [ForeignKey("UserId")]
    public User User { get; set; } = null!;
}