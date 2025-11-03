using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TiendaUcnApi.src.Application.DTO;
using TiendaUcnApi.src.Application.DTO.AuthDTO;
using TiendaUcnApi.src.Application.Services.Interfaces;

namespace TiendaUcnApi.src.API.Controllers;

/// <summary>
/// Controller for user profile management.
/// Allows viewing and updating profile, changing password, and verifying email changes.
/// </summary>
[ApiController]
[Route("api/user")]
[Authorize]
public class ProfileController : BaseController
{
    private readonly IProfileService _profileService;

    /// <summary>
    /// Initializes a new instance of the <see cref="ProfileController"/> class.
    /// </summary>
    /// <param name="profileService">The profile service for business logic.</param>
    public ProfileController(IProfileService profileService)
    {
        _profileService = profileService;
    }

    /// <summary>
    /// Retrieves the authenticated user identifier from the JWT token.
    /// </summary>
    /// <returns>User identifier.</returns>
    /// <exception cref="UnauthorizedAccessException">Thrown when user ID cannot be obtained from token.</exception>
    private int GetUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId))
        {
            throw new UnauthorizedAccessException("Could not obtain user identifier from token.");
        }
        return userId;
    }

    /// <summary>
    /// Retrieves the profile of the authenticated user.
    /// </summary>
    /// <returns>User profile information.</returns>
    /// <response code="200">Returns the user profile.</response>
    /// <response code="401">User not authenticated.</response>
    [HttpGet("profile")]
    public async Task<ActionResult<ProfileDTO>> GetProfile()
    {
        var profile = await _profileService.GetProfileAsync(GetUserId());
        return Ok(new GenericResponse<ProfileDTO>("Profile retrieved successfully", profile));
    }

    /// <summary>
    /// Updates the profile of the authenticated user.
    /// </summary>
    /// <param name="dto">Updated profile data.</param>
    /// <returns>Success message.</returns>
    /// <response code="200">Profile updated successfully.</response>
    /// <response code="400">Invalid data provided.</response>
    /// <response code="401">User not authenticated.</response>
    [HttpPut("profile")] // CORRECTED METHOD AND ROUTE
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileDTO dto)
    {
        var result = await _profileService.UpdateProfileAsync(GetUserId(), dto);
        return Ok(new GenericResponse<string>(result));
    }

    /// <summary>
    /// Changes the password of the authenticated user.
    /// </summary>
    /// <param name="dto">Current and new password.</param>
    /// <returns>Success message.</returns>
    /// <response code="200">Password changed successfully.</response>
    /// <response code="400">Invalid current password.</response>
    /// <response code="401">User not authenticated.</response>
    [HttpPatch("change-password")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDTO dto)
    {
        await _profileService.ChangePasswordAsync(GetUserId(), dto);
        return Ok(new GenericResponse<string>("Password changed successfully"));
    }

    /// <summary>
    /// Verifies an email address change for the authenticated user.
    /// </summary>
    /// <param name="dto">New email and verification code.</param>
    /// <returns>Success message.</returns>
    /// <response code="200">Email changed successfully.</response>
    /// <response code="400">Invalid or expired verification code.</response>
    /// <response code="401">User not authenticated.</response>
    [HttpPost("verify-email-change")]
    public async Task<IActionResult> VerifyEmailChange([FromBody] VerifyEmailChangeDTO dto)
    {
        var message = await _profileService.VerifyEmailChangeAsync(GetUserId(), dto);
        return Ok(new GenericResponse<string>(message));
    }
}
