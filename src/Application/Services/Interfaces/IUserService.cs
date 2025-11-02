using TiendaUcnApi.src.Application.DTO;
using TiendaUcnApi.src.Application.DTO.AuthDTO;

namespace TiendaUcnApi.src.Application.Services.Interfaces;

/// <summary>
/// Service interface for user authentication and management operations.
/// </summary>
public interface IUserService
{
    /// <summary>
    /// Authenticates a user and generates a JWT token.
    /// </summary>
    /// <param name="loginDTO">DTO containing user credentials.</param>
    /// <param name="httpContext">The current HTTP context.</param>
    /// <returns>A tuple containing the generated JWT token and the user ID.</returns>
    Task<(string token, int userId)> LoginAsync(LoginDTO loginDTO, HttpContext httpContext);

    /// <summary>
    /// Registers a new user in the system and sends email verification code.
    /// </summary>
    /// <param name="registerDTO">DTO containing new user information.</param>
    /// <param name="httpContext">The current HTTP context.</param>
    /// <returns>A success message indicating registration status.</returns>
    Task<string> RegisterAsync(RegisterDTO registerDTO, HttpContext httpContext);

    /// <summary>
    /// Verifies a user's email address using the provided verification code.
    /// </summary>
    /// <param name="verifyEmailDTO">DTO containing the email and verification code.</param>
    /// <returns>A success message indicating verification status.</returns>
    Task<string> VerifyEmailAsync(VerifyEmailDTO verifyEmailDTO);

    /// <summary>
    /// Resends the email verification code to the user.
    /// </summary>
    /// <param name="resendEmailVerificationCodeDTO">DTO containing the user's email address.</param>
    /// <returns>A success message indicating the code was resent.</returns>
    Task<string> ResendEmailVerificationCodeAsync(
        ResendEmailVerificationCodeDTO resendEmailVerificationCodeDTO
    );

    /// <summary>
    /// Deletes unconfirmed user accounts that have not verified their email.
    /// </summary>
    /// <returns>The number of deleted user accounts.</returns>
    Task<int> DeleteUnconfirmedAsync();

    /// <summary>
    /// Initiates the password recovery process by sending a reset code to the user's email.
    /// </summary>
    /// <param name="forgotPasswordDTO">DTO containing the user's email address.</param>
    /// <returns>A success message indicating the reset code was sent.</returns>
    Task<string> ForgotPasswordAsync(ForgotPasswordDTO forgotPasswordDTO);

    /// <summary>
    /// Resets a user's password using the provided verification code.
    /// </summary>
    /// <param name="resetPasswordDTO">DTO containing the email, code, and new password.</param>
    /// <returns>A success message indicating the password was reset.</returns>
    Task<string> ResetPasswordAsync(ResetPasswordDTO resetPasswordDTO);
}
