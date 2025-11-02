using Mapster;
using Microsoft.AspNetCore.Identity;
using Serilog;
using TiendaUcnApi.src.Application.DTO;
using TiendaUcnApi.src.Application.DTO.AuthDTO;
using TiendaUcnApi.src.Application.Services.Interfaces;
using TiendaUcnApi.src.Domain.Models;
using TiendaUcnApi.src.Infrastructure.Repositories.Interfaces;

namespace TiendaUcnApi.src.Application.Services.Implements;

/// <summary>
/// Service for user profile management.
/// Handles profile retrieval, updates, email changes, and password changes.
/// </summary>
public class ProfileService : IProfileService
{
    /// <summary>
    /// User repository.
    /// </summary>
    private readonly IUserRepository _userRepository;

    /// <summary>
    /// ASP.NET Core Identity user manager.
    /// </summary>
    private readonly UserManager<User> _userManager;

    /// <summary>
    /// Email service for sending notifications.
    /// </summary>
    private readonly IEmailService _emailService;

    /// <summary>
    /// Verification code repository.
    /// </summary>
    private readonly IVerificationCodeRepository _verificationCodeRepository;

    /// <summary>
    /// Application configuration.
    /// </summary>
    private readonly IConfiguration _configuration;

    /// <summary>
    /// Initializes a new instance of the <see cref="ProfileService"/> class with all necessary dependencies.
    /// </summary>
    /// <param name="userRepository">User repository.</param>
    /// <param name="userManager">User manager.</param>
    /// <param name="emailService">Email service.</param>
    /// <param name="verificationCodeRepository">Verification code repository.</param>
    /// <param name="configuration">Application configuration.</param>
    public ProfileService(
        IUserRepository userRepository,
        UserManager<User> userManager,
        IEmailService emailService,
        IVerificationCodeRepository verificationCodeRepository,
        IConfiguration configuration
    )
    {
        _userRepository = userRepository;
        _userManager = userManager;
        _emailService = emailService;
        _verificationCodeRepository = verificationCodeRepository;
        _configuration = configuration;
    }

    /// <summary>
    /// Retrieves the user's profile by user ID.
    /// </summary>
    /// <param name="userId">User ID.</param>
    /// <returns>DTO with profile data.</returns>
    public async Task<ProfileDTO> GetProfileAsync(int userId)
    {
        var user =
            await _userRepository.GetByIdAsync(userId)
            ?? throw new KeyNotFoundException("User not found.");
        return user.Adapt<ProfileDTO>();
    }

    /// <summary>
    /// Updates the user's profile data.
    /// </summary>
    /// <param name="userId">User ID.</param>
    /// <param name="dto">DTO with data to update.</param>
    /// <returns>Confirmation message.</returns>
    public async Task<string> UpdateProfileAsync(int userId, UpdateProfileDTO dto)
    {
        var user =
            await _userRepository.GetByIdAsync(userId)
            ?? throw new KeyNotFoundException("User not found.");
        var changesMade = false;

        if (
            !string.IsNullOrEmpty(dto.Email)
            && !dto.Email.Equals(user.Email, StringComparison.OrdinalIgnoreCase)
        )
        {
            if (await _userRepository.ExistsByEmailAsync(dto.Email))
            {
                throw new ArgumentException("The email address is already in use by another user.");
            }
            var code = new Random().Next(100000, 999999).ToString();
            var verificationCode = new VerificationCode
            {
                UserId = user.Id,
                Code = code,
                CodeType = CodeType.EmailChange,
                ExpiryDate = DateTime.UtcNow.AddMinutes(10),
            };
            await _verificationCodeRepository.CreateAsync(verificationCode);
            await _emailService.SendVerificationCodeEmailAsync(dto.Email, code);
            Log.Information(
                "User {UserId} requested email change to {NewEmail}. Verification code sent.",
                userId,
                dto.Email
            );
        }

        if (
            !string.IsNullOrEmpty(dto.Rut)
            && !dto.Rut.Equals(user.Rut, StringComparison.OrdinalIgnoreCase)
        )
        {
            if (await _userRepository.ExistsByRutAsync(dto.Rut))
                throw new ArgumentException("El RUT ya está en uso.");
            user.Rut = dto.Rut;
            changesMade = true;
        }

        if (dto.FirstName != null && user.FirstName != dto.FirstName)
        {
            user.FirstName = dto.FirstName;
            changesMade = true;
        }
        if (dto.LastName != null && user.LastName != dto.LastName)
        {
            user.LastName = dto.LastName;
            changesMade = true;
        }
        if (dto.PhoneNumber != null && user.PhoneNumber != dto.PhoneNumber)
        {
            user.PhoneNumber = dto.PhoneNumber;
            changesMade = true;
        }
        if (dto.Gender != null && user.Gender.ToString() != dto.Gender)
        {
            user.Gender = Enum.Parse<Gender>(dto.Gender);
            changesMade = true;
        }
        if (dto.BirthDate.HasValue && user.BirthDate != dto.BirthDate.Value)
        {
            user.BirthDate = dto.BirthDate.Value;
            changesMade = true;
        }

        if (changesMade)
        {
            user.UpdatedAt = DateTime.UtcNow;
            await _userManager.UpdateAsync(user);
            Log.Information("User profile {UserId} updated (non-email related data).", userId);
        }

        return "Your data has been updated. If you requested an email change, please verify the code sent to your new address.";
    }

    /// <summary>
    /// Verifies the user's email change.
    /// Validates the verification code and updates the email if valid.
    /// </summary>
    /// <param name="userId">User ID.</param>
    /// <param name="dto">DTO containing the new email and verification code.</param>
    /// <returns>Confirmation message.</returns>
    /// <exception cref="KeyNotFoundException">Thrown when user is not found.</exception>
    /// <exception cref="ArgumentException">Thrown when the verification code is incorrect or expired.</exception>
    /// <exception cref="Exception">Thrown when an error occurs during email update.</exception>
    public async Task<string> VerifyEmailChangeAsync(int userId, VerifyEmailChangeDTO dto)
    {
        var user =
            await _userRepository.GetByIdAsync(userId)
            ?? throw new KeyNotFoundException("User not found.");
        var verificationCode = await _verificationCodeRepository.GetLatestByUserIdAsync(
            user.Id,
            CodeType.EmailChange
        );

        if (
            verificationCode == null
            || verificationCode.Code != dto.Code
            || DateTime.UtcNow >= verificationCode.ExpiryDate
        )
        {
            throw new ArgumentException("The verification code is incorrect or has expired.");
        }

        user.Email = dto.NewEmail;
        user.UserName = dto.NewEmail;
        user.NormalizedEmail = _userManager.KeyNormalizer.NormalizeEmail(dto.NewEmail);
        user.NormalizedUserName = _userManager.KeyNormalizer.NormalizeName(dto.NewEmail);
        user.EmailConfirmed = true;

        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
        {
            throw new Exception("An error occurred while updating your email address.");
        }

        await _verificationCodeRepository.DeleteByUserIdAsync(user.Id, CodeType.EmailChange);
        Log.Information(
            "User {UserId} has successfully confirmed their new email: {NewEmail}",
            userId,
            dto.NewEmail
        );
        return "Your email address has been successfully updated.";
    }

    /// <summary>
    /// Changes the user's password.
    /// Validates the old password, updates to the new password, and invalidates all existing sessions.
    /// </summary>
    /// <param name="userId">User ID.</param>
    /// <param name="dto">DTO containing the current password and the new password.</param>
    /// <exception cref="KeyNotFoundException">Thrown when user is not found.</exception>
    /// <exception cref="ArgumentException">Thrown when password change fails (invalid old password, weak new password, etc.).</exception>
    public async Task ChangePasswordAsync(int userId, ChangePasswordDTO dto)
    {
        var user =
            await _userRepository.GetByIdAsync(userId)
            ?? throw new KeyNotFoundException("User not found.");
        var result = await _userManager.ChangePasswordAsync(user, dto.OldPassword, dto.NewPassword);

        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            Log.Warning("Password change failed for user {UserId}: {Errors}", userId, errors);
            throw new ArgumentException($"Error changing password: {errors}");
        }

        await _userManager.UpdateSecurityStampAsync(user);

        Log.Information(
            "Password for user {UserId} changed successfully and sessions invalidated.",
            userId
        );
    }
}
