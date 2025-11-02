using TiendaUcnApi.src.Domain.Models;

namespace TiendaUcnApi.src.Infrastructure.Repositories.Interfaces;

/// <summary>
/// Repository interface for verification code data access operations.
/// Manages verification codes for email verification and password reset.
/// </summary>
public interface IVerificationCodeRepository
{
    /// <summary>
    /// Creates a new verification code in the database.
    /// </summary>
    /// <param name="verificationCode">The verification code entity to create.</param>
    /// <returns>The created verification code entity.</returns>
    Task<VerificationCode> CreateAsync(VerificationCode verificationCode);

    /// <summary>
    /// Retrieves the most recent verification code for a user by code type.
    /// </summary>
    /// <param name="userId">The user identifier.</param>
    /// <param name="codeType">The type of verification code (EmailVerification or PasswordReset).</param>
    /// <returns>The latest verification code or null if not found.</returns>
    Task<VerificationCode?> GetLatestByUserIdAsync(int userId, CodeType codeType);

    /// <summary>
    /// Increments the attempt counter for a verification code.
    /// Used to track failed verification attempts.
    /// </summary>
    /// <param name="userId">The user identifier.</param>
    /// <param name="codeType">The type of verification code.</param>
    /// <returns>The updated attempt count.</returns>
    Task<int> IncreaseAttemptsAsync(int userId, CodeType codeType);

    /// <summary>
    /// Deletes a specific verification code by user ID and code type.
    /// </summary>
    /// <param name="id">The user identifier.</param>
    /// <param name="codeType">The type of verification code to delete.</param>
    /// <returns>True if deletion was successful, false if code didn't exist.</returns>
    Task<bool> DeleteByUserIdAsync(int id, CodeType codeType);

    /// <summary>
    /// Updates an existing verification code in the database.
    /// </summary>
    /// <param name="verificationCode">The verification code entity with updated data.</param>
    /// <returns>The updated verification code or null if not found.</returns>
    Task<VerificationCode?> UpdateAsync(VerificationCode verificationCode);

    /// <summary>
    /// Deletes all verification codes associated with a user.
    /// Used when deleting a user account.
    /// </summary>
    /// <param name="userId">The user identifier.</param>
    /// <returns>Number of verification codes deleted.</returns>
    Task<int> DeleteByUserIdAsync(int userId);
}
