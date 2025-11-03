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
/// User service implementation.
/// Handles user authentication, registration, email verification, and password reset operations.
/// </summary>
public class UserService : IUserService
{
    /// <summary>
    /// Service for JWT token generation.
    /// </summary>
    private readonly ITokenService _tokenService;

    /// <summary>
    /// User repository.
    /// </summary>
    private readonly IUserRepository _userRepository;

    /// <summary>
    /// Service for sending emails.
    /// </summary>
    private readonly IEmailService _emailService;

    /// <summary>
    /// Application configuration.
    /// </summary>
    private readonly IConfiguration _configuration;

    /// <summary>
    /// Verification code repository.
    /// </summary>
    private readonly IVerificationCodeRepository _verificationCodeRepository;

    /// <summary>
    /// Identity user manager.
    /// </summary>
    private readonly UserManager<User> _userManager;

    /// <summary>
    /// Verification code expiration time in minutes.
    /// </summary>
    private readonly int _verificationCodeExpirationTimeInMinutes;

    /// <summary>
    /// Initializes a new instance of the UserService class with all necessary dependencies.
    /// </summary>
    /// <param name="tokenService">Token service.</param>
    /// <param name="userRepository">User repository.</param>
    /// <param name="emailService">Email service.</param>
    /// <param name="verificationCodeRepository">Verification code repository.</param>
    /// <param name="configuration">Application configuration.</param>
    /// <param name="userManager">User manager.</param>
    public UserService(
        ITokenService tokenService,
        IUserRepository userRepository,
        IEmailService emailService,
        IVerificationCodeRepository verificationCodeRepository,
        IConfiguration configuration,
        UserManager<User> userManager
    )
    {
        _tokenService = tokenService;
        _userRepository = userRepository;
        _emailService = emailService;
        _verificationCodeRepository = verificationCodeRepository;
        _configuration = configuration;
        _userManager = userManager;
        _verificationCodeExpirationTimeInMinutes = _configuration.GetValue<int>(
            "VerificationCode:ExpirationTimeInMinutes"
        );
    }

    /// <summary>
    /// Logs in a user with the provided credentials.
    /// Validates email confirmation and password, generates JWT token.
    /// </summary>
    /// <param name="loginDTO">DTO containing login credentials.</param>
    /// <param name="httpContext">Current HTTP context for IP address logging.</param>
    /// <returns>Tuple containing JWT token and user ID.</returns>
    /// <exception cref="UnauthorizedAccessException">Thrown when credentials are invalid.</exception>
    /// <exception cref="InvalidOperationException">Thrown when email is not confirmed or user has no role.</exception>
    public async Task<(string token, int userId)> LoginAsync(
        LoginDTO loginDTO,
        HttpContext httpContext
    )
    {
        var ipAddress = httpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown IP";
        var user = await _userRepository.GetByEmailAsync(loginDTO.Email);

        if (user == null)
        {
            Log.Warning($"Failed login attempt for user: {loginDTO.Email} from IP: {ipAddress}");
            throw new UnauthorizedAccessException("Invalid credentials.");
        }

        if (!user.EmailConfirmed)
        {
            Log.Warning(
                $"Failed login attempt for user: {loginDTO.Email} from IP: {ipAddress} - Email not confirmed."
            );
            throw new InvalidOperationException("The user's email has not been confirmed.");
        }

        var result = await _userRepository.CheckPasswordAsync(user, loginDTO.Password);
        if (!result)
        {
            Log.Warning($"Failed login attempt for user: {loginDTO.Email} from IP: {ipAddress}");
            throw new UnauthorizedAccessException("Invalid credentials.");
        }

        string roleName =
            await _userRepository.GetUserRoleAsync(user)
            ?? throw new InvalidOperationException("The user does not have an assigned role.");

        Log.Information($"Successful login for user: {loginDTO.Email} from IP: {ipAddress}");
        var token = _tokenService.GenerateToken(user, roleName, loginDTO.RememberMe);
        return (token, user.Id);
    }

    /// <summary>
    /// Registers a new user.
    /// Validates email and RUT uniqueness, creates user with "Cliente" role, generates verification code.
    /// </summary>
    /// <param name="registerDTO">DTO containing registration data.</param>
    /// <param name="httpContext">Current HTTP context for IP address logging.</param>
    /// <returns>Confirmation message.</returns>
    /// <exception cref="InvalidOperationException">Thrown when email or RUT already exists.</exception>
    /// <exception cref="Exception">Thrown when user creation fails.</exception>
    public async Task<string> RegisterAsync(RegisterDTO registerDTO, HttpContext httpContext)
    {
        var ipAddress = httpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown IP";
        Log.Information($"New user registration attempt: {registerDTO.Email} from IP: {ipAddress}");

        if (await _userRepository.ExistsByEmailAsync(registerDTO.Email))
        {
            Log.Warning($"User with email {registerDTO.Email} is already registered.");
            throw new InvalidOperationException("The user is already registered.");
        }
        if (await _userRepository.ExistsByRutAsync(registerDTO.Rut))
        {
            Log.Warning($"User with RUT {registerDTO.Rut} is already registered.");
            throw new InvalidOperationException("The RUT is already registered.");
        }

        var user = registerDTO.Adapt<User>();
        var result = await _userRepository.CreateAsync(user, registerDTO.Password);
        if (!result)
        {
            Log.Warning($"Error registering user: {registerDTO.Email}");
            throw new Exception("Error registering the user.");
        }

        Log.Information(
            $"Successful registration for user: {registerDTO.Email} from IP: {ipAddress}"
        );
        string code = new Random().Next(100000, 999999).ToString();
        var verificationCode = new VerificationCode
        {
            UserId = user.Id,
            Code = code,
            CodeType = CodeType.EmailVerification,
            ExpiryDate = DateTime.UtcNow.AddMinutes(_verificationCodeExpirationTimeInMinutes),
        };

        var createdVerificationCode = await _verificationCodeRepository.CreateAsync(
            verificationCode
        );
        Log.Information(
            $"Verification code generated for user: {registerDTO.Email} - Code: {createdVerificationCode.Code}"
        );

        await _emailService.SendVerificationCodeEmailAsync(
            registerDTO.Email,
            createdVerificationCode.Code
        );
        Log.Information($"Verification code sent to email: {registerDTO.Email}");
        return "A verification code has been sent to your email address.";
    }

    /// <summary>
    /// Resends the verification code to the user's email.
    /// Validates cooldown period before allowing code regeneration.
    /// </summary>
    /// <param name="resendEmailVerificationCodeDTO">DTO containing user's email.</param>
    /// <returns>Confirmation message.</returns>
    /// <exception cref="KeyNotFoundException">Thrown when user doesn't exist.</exception>
    /// <exception cref="InvalidOperationException">Thrown when email is already verified.</exception>
    /// <exception cref="TimeoutException">Thrown when cooldown period hasn't elapsed.</exception>
    public async Task<string> ResendEmailVerificationCodeAsync(
        ResendEmailVerificationCodeDTO resendEmailVerificationCodeDTO
    )
    {
        var currentTime = DateTime.UtcNow;
        User? user = await _userRepository.GetByEmailAsync(resendEmailVerificationCodeDTO.Email);
        if (user == null)
        {
            Log.Warning($"User with email {resendEmailVerificationCodeDTO.Email} does not exist.");
            throw new KeyNotFoundException("The user does not exist.");
        }
        if (user.EmailConfirmed)
        {
            Log.Warning(
                $"User with email {resendEmailVerificationCodeDTO.Email} has already verified their email."
            );
            throw new InvalidOperationException("The email has already been verified.");
        }

        VerificationCode? verificationCode =
            await _verificationCodeRepository.GetLatestByUserIdAsync(
                user.Id,
                CodeType.EmailVerification
            );
        var expirationTime = verificationCode!.CreatedAt.AddMinutes(
            _verificationCodeExpirationTimeInMinutes
        );
        if (expirationTime > currentTime)
        {
            int remainingSeconds = (int)(expirationTime - currentTime).TotalSeconds;
            Log.Warning(
                $"User {resendEmailVerificationCodeDTO.Email} requested code resend before {_verificationCodeExpirationTimeInMinutes} minutes elapsed."
            );
            throw new TimeoutException(
                $"You must wait {remainingSeconds} seconds to request a new verification code."
            );
        }

        string newCode = new Random().Next(100000, 999999).ToString();
        verificationCode.Code = newCode;
        verificationCode.ExpiryDate = DateTime.UtcNow.AddMinutes(
            _verificationCodeExpirationTimeInMinutes
        );
        await _verificationCodeRepository.UpdateAsync(verificationCode);

        Log.Information(
            $"New verification code generated for user: {resendEmailVerificationCodeDTO.Email} - Code: {newCode}"
        );
        await _emailService.SendVerificationCodeEmailAsync(user.Email!, newCode);
        Log.Information(
            $"New verification code resent to email: {resendEmailVerificationCodeDTO.Email}"
        );
        return "A new verification code has been sent to your email address.";
    }

    /// <summary>
    /// Verifies a user's email using the provided verification code.
    /// Limits to 5 verification attempts; automatically deletes user and code after exceeding limit.
    /// </summary>
    /// <param name="verifyEmailDTO">DTO containing email and verification code.</param>
    /// <returns>Confirmation message with login instructions.</returns>
    /// <exception cref="KeyNotFoundException">Thrown when user or verification code doesn't exist.</exception>
    /// <exception cref="InvalidOperationException">Thrown when email is already verified.</exception>
    /// <exception cref="ArgumentException">Thrown when code is invalid/expired or attempts exceeded.</exception>
    /// <exception cref="Exception">Thrown when email confirmation fails.</exception>
    public async Task<string> VerifyEmailAsync(VerifyEmailDTO verifyEmailDTO)
    {
        User? user = await _userRepository.GetByEmailAsync(verifyEmailDTO.Email);
        if (user == null)
        {
            Log.Warning($"User with email {verifyEmailDTO.Email} does not exist.");
            throw new KeyNotFoundException("The user does not exist.");
        }
        if (user.EmailConfirmed)
        {
            Log.Warning(
                $"User with email {verifyEmailDTO.Email} has already verified their email."
            );
            throw new InvalidOperationException("The email has already been verified.");
        }

        CodeType codeType = CodeType.EmailVerification;
        VerificationCode? verificationCode =
            await _verificationCodeRepository.GetLatestByUserIdAsync(user.Id, codeType);
        if (verificationCode == null)
        {
            Log.Warning($"Verification code not found for user: {verifyEmailDTO.Email}");
            throw new KeyNotFoundException("The verification code does not exist.");
        }

        if (
            verificationCode.Code != verifyEmailDTO.VerificationCode
            || DateTime.UtcNow >= verificationCode.ExpiryDate
        )
        {
            int attempsCountUpdated = await _verificationCodeRepository.IncreaseAttemptsAsync(
                user.Id,
                codeType
            );
            Log.Warning(
                $"Incorrect or expired verification code for user: {verifyEmailDTO.Email}. Current attempts: {attempsCountUpdated}"
            );

            if (attempsCountUpdated >= 5)
            {
                Log.Warning($"Attempt limit reached for user: {verifyEmailDTO.Email}");
                if (await _verificationCodeRepository.DeleteByUserIdAsync(user.Id, codeType))
                {
                    Log.Warning($"Verification code deleted for user: {verifyEmailDTO.Email}");
                    if (await _userRepository.DeleteAsync(user.Id))
                    {
                        Log.Warning($"User deleted: {verifyEmailDTO.Email}");
                        throw new ArgumentException(
                            "The attempt limit has been reached. The user has been deleted."
                        );
                    }
                }
            }

            if (DateTime.UtcNow >= verificationCode.ExpiryDate)
            {
                Log.Warning($"Verification code has expired for user: {verifyEmailDTO.Email}");
                throw new ArgumentException("The verification code has expired.");
            }
            else
            {
                Log.Warning($"Incorrect verification code for user: {verifyEmailDTO.Email}");
                throw new ArgumentException(
                    $"The verification code is incorrect, {5 - attempsCountUpdated} attempts remaining."
                );
            }
        }

        if (await _userRepository.ConfirmEmailAsync(user.Email!))
        {
            if (await _verificationCodeRepository.DeleteByUserIdAsync(user.Id, codeType))
            {
                Log.Warning($"Verification code deleted for user: {verifyEmailDTO.Email}");
                await _emailService.SendWelcomeEmailAsync(user.Email!);
                Log.Information($"Email successfully confirmed for user {verifyEmailDTO.Email}.");
                return "You can now log in and enjoy all the benefits of Tienda UCN!";
            }
            throw new Exception("Error confirming email.");
        }
        throw new Exception("Error verifying email.");
    }

    /// <summary>
    /// Deletes unconfirmed users.
    /// Used for cleanup of users who haven't verified their email within the allowed time.
    /// </summary>
    /// <returns>Number of deleted users.</returns>
    public async Task<int> DeleteUnconfirmedAsync()
    {
        return await _userRepository.DeleteUnconfirmedAsync();
    }

    /// <summary>
    /// Requests a password reset by sending a verification code to the user's email.
    /// Generates a 6-digit code valid for the configured expiration time.
    /// For security, returns same message whether user exists or not.
    /// </summary>
    /// <param name="forgotPasswordDTO">DTO containing the user's email.</param>
    /// <returns>Generic confirmation message (doesn't reveal if user exists).</returns>
    public async Task<string> ForgotPasswordAsync(ForgotPasswordDTO forgotPasswordDTO)
    {
        var user = await _userManager.FindByEmailAsync(forgotPasswordDTO.Email);
        if (user == null || !user.EmailConfirmed)
        {
            // Don't reveal that the user doesn't exist for better security.
            return "If an account exists for this email, a password reset code has been sent.";
        }

        // Generate, save, and send a 6-digit code.
        string code = new Random().Next(100000, 999999).ToString();
        var verificationCode = new VerificationCode
        {
            UserId = user.Id,
            Code = code,
            CodeType = CodeType.PasswordReset, // Important to use the correct type
            ExpiryDate = DateTime.UtcNow.AddMinutes(_verificationCodeExpirationTimeInMinutes),
        };

        await _verificationCodeRepository.CreateAsync(verificationCode);
        Log.Information("Password reset code generated for {Email}", user.Email);

        // Call the email service method
        await _emailService.SendPasswordResetCodeEmailAsync(user.Email!, user.FirstName, code);

        return "If an account exists for this email, a password reset code has been sent.";
    }

    /// <summary>
    /// Resets a user's password using a 6-digit verification code.
    /// Validates the code and updates the password using Identity's secure password hashing.
    /// For security, returns same message whether user exists or not.
    /// </summary>
    /// <param name="resetPasswordDTO">DTO containing email, verification code, and new password.</param>
    /// <returns>Generic confirmation message (doesn't reveal if user exists).</returns>
    /// <exception cref="ArgumentException">Thrown when code is invalid/expired or password doesn't meet requirements.</exception>
    public async Task<string> ResetPasswordAsync(ResetPasswordDTO resetPasswordDTO)
    {
        var user = await _userManager.FindByEmailAsync(resetPasswordDTO.Email);
        if (user == null)
        {
            // Don't reveal that the user doesn't exist.
            return "The password has been successfully reset.";
        }

        // Find and validate the 6-digit code
        var verificationCode = await _verificationCodeRepository.GetLatestByUserIdAsync(
            user.Id,
            CodeType.PasswordReset
        );

        if (
            verificationCode == null
            || verificationCode.Code != resetPasswordDTO.Code
            || DateTime.UtcNow >= verificationCode.ExpiryDate
        )
        {
            Log.Warning(
                "Password reset attempt with invalid or expired code for {Email}",
                resetPasswordDTO.Email
            );
            throw new ArgumentException("The reset code is incorrect or has expired.");
        }

        // If the code is valid, proceed to change the password.
        // UserManager requires a reset token, so we generate it internally just before using it.
        var internalToken = await _userManager.GeneratePasswordResetTokenAsync(user);
        var result = await _userManager.ResetPasswordAsync(
            user,
            internalToken,
            resetPasswordDTO.NewPassword
        );

        if (result.Succeeded)
        {
            // It's good practice to delete the code once it has been used.
            await _verificationCodeRepository.DeleteByUserIdAsync(user.Id, CodeType.PasswordReset);
            Log.Information("Password successfully reset for {Email}", resetPasswordDTO.Email);
            return "The password has been successfully reset.";
        }

        // If it fails (e.g., the new password doesn't meet Identity policies), throw the errors.
        var errorMessages = string.Join(" ", result.Errors.Select(e => e.Description));
        Log.Warning(
            "Password reset failed for {Email}: {Errors}",
            resetPasswordDTO.Email,
            errorMessages
        );
        throw new ArgumentException(errorMessages);
    }
}
