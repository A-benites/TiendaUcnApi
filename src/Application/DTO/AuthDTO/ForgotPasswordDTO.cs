using System.ComponentModel.DataAnnotations;

/// <summary>
/// Data Transfer Object for requesting password reset.
/// </summary>
public class ForgotPasswordDTO
{
    /// <summary>
    /// User's email address where the password reset code will be sent.
    /// </summary>
    [EmailAddress]
    public required string Email { get; set; }
}
