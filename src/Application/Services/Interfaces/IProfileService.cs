using TiendaUcnApi.src.Application.DTO;
using TiendaUcnApi.src.Application.DTO.AuthDTO;

namespace TiendaUcnApi.src.Application.Services.Interfaces;

/// <summary>
/// Service interface for user profile management operations.
/// </summary>
public interface IProfileService
{
    /// <summary>
    /// Retrieves the profile information for the specified user.
    /// </summary>
    /// <param name="userId">The user identifier.</param>
    /// <returns>DTO containing the user's profile data.</returns>
    Task<ProfileDTO> GetProfileAsync(int userId);

    /// <summary>
    /// Updates the profile information for the specified user.
    /// </summary>
    /// <param name="userId">The user identifier to update.</param>
    /// <param name="dto">DTO containing updated profile data.</param>
    /// <returns>Success message indicating the profile was updated.</returns>
    Task<string> UpdateProfileAsync(int userId, UpdateProfileDTO dto);

    /// <summary>
    /// Changes the password for the specified user.
    /// </summary>
    /// <param name="userId">The user identifier.</param>
    /// <param name="dto">DTO containing current and new password.</param>
    Task ChangePasswordAsync(int userId, ChangePasswordDTO dto);

    /// <summary>
    /// Verifies an email address change using a verification code.
    /// </summary>
    /// <param name="userId">The user identifier.</param>
    /// <param name="dto">DTO containing the new email and verification code.</param>
    /// <returns>Success message indicating the email was verified and changed.</returns>
    Task<string> VerifyEmailChangeAsync(int userId, VerifyEmailChangeDTO dto);
}
