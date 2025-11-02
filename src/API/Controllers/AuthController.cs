using Microsoft.AspNetCore.Mvc;
using Serilog;
using TiendaUcnApi.src.Application.DTO;
using TiendaUcnApi.src.Application.DTO.AuthDTO;
using TiendaUcnApi.src.Application.Services.Interfaces;

namespace TiendaUcnApi.src.API.Controllers;

/// <summary>
/// Controller for user authentication and registration operations.
/// Handles login, registration, email verification, and password recovery.
/// </summary>
public class AuthController(IUserService userService) : BaseController
{
    private readonly IUserService _userService = userService;

    /// <summary>
    /// Authenticates a user and returns a JWT token.
    /// Associates anonymous cart with authenticated user if applicable.
    /// </summary>
    /// <param name="loginDTO">Login credentials including email and password.</param>
    /// <returns>JWT token for authenticated requests.</returns>
    /// <response code="200">Returns the authentication token.</response>
    /// <response code="401">Invalid credentials or unverified email.</response>
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDTO loginDTO)
    {
        var (token, userId) = await _userService.LoginAsync(loginDTO, HttpContext);
        var buyerId = HttpContext.Items["BuyerId"]?.ToString();
        if (!string.IsNullOrEmpty(buyerId))
        {
            Log.Information(
                "Cart associated with user. BuyerId: {BuyerId}, UserId: {UserId}",
                buyerId,
                userId
            );
        }
        return Ok(new GenericResponse<string>("Successful login", token));
    }

    /// <summary>
    /// Registers a new user account and sends email verification code.
    /// </summary>
    /// <param name="registerDTO">User registration data including personal information.</param>
    /// <returns>Success message with verification instructions.</returns>
    /// <response code="201">User created successfully.</response>
    /// <response code="409">Email or RUT already registered.</response>
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDTO registerDTO)
    {
        var message = await _userService.RegisterAsync(registerDTO, HttpContext);
        return Created(
            "/api/user/profile",
            new GenericResponse<string>("Successful registration", message)
        );
    }

    /// <summary>
    /// Verifies a user's email address using the verification code.
    /// </summary>
    /// <param name="verifyEmailDTO">Email and verification code.</param>
    /// <returns>Success message indicating email verification.</returns>
    /// <response code="200">Email verified successfully.</response>
    /// <response code="400">Invalid or expired verification code.</response>
    [HttpPost("verify")]
    public async Task<IActionResult> VerifyEmail([FromBody] VerifyEmailDTO verifyEmailDTO)
    {
        var message = await _userService.VerifyEmailAsync(verifyEmailDTO);
        return Ok(new GenericResponse<string>("Successful email verification", message));
    }

    /// <summary>
    /// Resends the email verification code to the user.
    /// </summary>
    /// <param name="resendEmailVerificationCodeDTO">User's email address.</param>
    /// <returns>Success message indicating code was resent.</returns>
    /// <response code="200">Verification code resent successfully.</response>
    /// <response code="404">User not found.</response>
    [HttpPost("resend-email-verification-code")]
    public async Task<IActionResult> ResendEmailVerificationCode(
        [FromBody] ResendEmailVerificationCodeDTO resendEmailVerificationCodeDTO
    )
    {
        var message = await _userService.ResendEmailVerificationCodeAsync(
            resendEmailVerificationCodeDTO
        );
        return Ok(new GenericResponse<string>("Verification code resent successfully", message));
    }

    /// <summary>
    /// Initiates the password recovery process by sending a reset code to the user's email.
    /// </summary>
    /// <param name="forgotPasswordDTO">User's email address.</param>
    /// <returns>Success message indicating reset code was sent.</returns>
    /// <response code="200">Reset code sent successfully.</response>
    /// <response code="404">User not found.</response>
    [HttpPost("recover-password")]
    public async Task<IActionResult> RecoverPassword([FromBody] ForgotPasswordDTO forgotPasswordDTO)
    {
        var message = await _userService.ForgotPasswordAsync(forgotPasswordDTO);
        return Ok(new GenericResponse<string>("Reset request sent", message));
    }

    /// <summary>
    /// Resets a user's password using the verification code.
    /// </summary>
    /// <param name="resetPasswordDTO">Email, verification code, and new password.</param>
    /// <returns>Success message indicating password was reset.</returns>
    /// <response code="200">Password reset successfully.</response>
    /// <response code="400">Invalid or expired reset code.</response>
    [HttpPatch("reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDTO resetPasswordDTO)
    {
        var message = await _userService.ResetPasswordAsync(resetPasswordDTO);
        return Ok(new GenericResponse<string>("Password reset", message));
    }
}
