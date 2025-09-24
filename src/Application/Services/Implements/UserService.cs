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
/// Implementación del servicio de usuarios.
/// </summary>
public class UserService : IUserService
{
    private readonly ITokenService _tokenService;
    private readonly IUserRepository _userRepository;
    private readonly IEmailService _emailService;
    private readonly IConfiguration _configuration;
    private readonly IVerificationCodeRepository _verificationCodeRepository;
    private readonly UserManager<User> _userManager;
    private readonly int _verificationCodeExpirationTimeInMinutes;

    // Constructor unificado con todas las dependencias necesarias
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
    /// Inicia sesión con el usuario proporcionado.
    /// </summary>
    public async Task<(string token, int userId)> LoginAsync(
        LoginDTO loginDTO,
        HttpContext httpContext
    )
    {
        var ipAddress = httpContext.Connection.RemoteIpAddress?.ToString() ?? "IP desconocida";
        var user = await _userRepository.GetByEmailAsync(loginDTO.Email);

        if (user == null)
        {
            Log.Warning(
                $"Intento de inicio de sesión fallido para el usuario: {loginDTO.Email} desde la IP: {ipAddress}"
            );
            throw new UnauthorizedAccessException("Credenciales inválidas.");
        }

        if (!user.EmailConfirmed)
        {
            Log.Warning(
                $"Intento de inicio de sesión fallido para el usuario: {loginDTO.Email} desde la IP: {ipAddress} - Correo no confirmado."
            );
            throw new InvalidOperationException(
                "El correo electrónico del usuario no ha sido confirmado."
            );
        }

        var result = await _userRepository.CheckPasswordAsync(user, loginDTO.Password);
        if (!result)
        {
            Log.Warning(
                $"Intento de inicio de sesión fallido para el usuario: {loginDTO.Email} desde la IP: {ipAddress}"
            );
            throw new UnauthorizedAccessException("Credenciales inválidas.");
        }

        string roleName =
            await _userRepository.GetUserRoleAsync(user)
            ?? throw new InvalidOperationException("El usuario no tiene un rol asignado.");

        Log.Information(
            $"Inicio de sesión exitoso para el usuario: {loginDTO.Email} desde la IP: {ipAddress}"
        );
        var token = _tokenService.GenerateToken(user, roleName, loginDTO.RememberMe);
        return (token, user.Id);
    }

    /// <summary>
    /// Registra un nuevo usuario.
    /// </summary>
    public async Task<string> RegisterAsync(RegisterDTO registerDTO, HttpContext httpContext)
    {
        var ipAddress = httpContext.Connection.RemoteIpAddress?.ToString() ?? "IP desconocida";
        Log.Information(
            $"Intento de registro de nuevo usuario: {registerDTO.Email} desde la IP: {ipAddress}"
        );

        if (await _userRepository.ExistsByEmailAsync(registerDTO.Email))
        {
            Log.Warning($"El usuario con el correo {registerDTO.Email} ya está registrado.");
            throw new InvalidOperationException("El usuario ya está registrado.");
        }
        if (await _userRepository.ExistsByRutAsync(registerDTO.Rut))
        {
            Log.Warning($"El usuario con el RUT {registerDTO.Rut} ya está registrado.");
            throw new InvalidOperationException("El RUT ya está registrado.");
        }

        var user = registerDTO.Adapt<User>();
        var result = await _userRepository.CreateAsync(user, registerDTO.Password);
        if (!result)
        {
            Log.Warning($"Error al registrar el usuario: {registerDTO.Email}");
            throw new Exception("Error al registrar el usuario.");
        }

        Log.Information(
            $"Registro exitoso para el usuario: {registerDTO.Email} desde la IP: {ipAddress}"
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
            $"Código de verificación generado para el usuario: {registerDTO.Email} - Código: {createdVerificationCode.Code}"
        );

        await _emailService.SendVerificationCodeEmailAsync(
            registerDTO.Email,
            createdVerificationCode.Code
        );
        Log.Information(
            $"Se ha enviado un código de verificación al correo electrónico: {registerDTO.Email}"
        );
        return "Se ha enviado un código de verificación a su correo electrónico.";
    }

    /// <summary>
    /// Reenvía el código de verificación al correo electrónico del usuario.
    /// </summary>
    public async Task<string> ResendEmailVerificationCodeAsync(
        ResendEmailVerificationCodeDTO resendEmailVerificationCodeDTO
    )
    {
        var currentTime = DateTime.UtcNow;
        User? user = await _userRepository.GetByEmailAsync(resendEmailVerificationCodeDTO.Email);
        if (user == null)
        {
            Log.Warning(
                $"El usuario con el correo {resendEmailVerificationCodeDTO.Email} no existe."
            );
            throw new KeyNotFoundException("El usuario no existe.");
        }
        if (user.EmailConfirmed)
        {
            Log.Warning(
                $"El usuario con el correo {resendEmailVerificationCodeDTO.Email} ya ha verificado su correo electrónico."
            );
            throw new InvalidOperationException("El correo electrónico ya ha sido verificado.");
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
                $"El usuario {resendEmailVerificationCodeDTO.Email} ha solicitado un reenvío del código de verificación antes de los {_verificationCodeExpirationTimeInMinutes} minutos."
            );
            throw new TimeoutException(
                $"Debe esperar {remainingSeconds} segundos para solicitar un nuevo código de verificación."
            );
        }

        string newCode = new Random().Next(100000, 999999).ToString();
        verificationCode.Code = newCode;
        verificationCode.ExpiryDate = DateTime.UtcNow.AddMinutes(
            _verificationCodeExpirationTimeInMinutes
        );
        await _verificationCodeRepository.UpdateAsync(verificationCode);

        Log.Information(
            $"Nuevo código de verificación generado para el usuario: {resendEmailVerificationCodeDTO.Email} - Código: {newCode}"
        );
        await _emailService.SendVerificationCodeEmailAsync(user.Email!, newCode);
        Log.Information(
            $"Se ha reenviado un nuevo código de verificación al correo electrónico: {resendEmailVerificationCodeDTO.Email}"
        );
        return "Se ha reenviado un nuevo código de verificación a su correo electrónico.";
    }

    /// <summary>
    /// Verifica el correo electrónico del usuario.
    /// </summary>
    public async Task<string> VerifyEmailAsync(VerifyEmailDTO verifyEmailDTO)
    {
        User? user = await _userRepository.GetByEmailAsync(verifyEmailDTO.Email);
        if (user == null)
        {
            Log.Warning($"El usuario con el correo {verifyEmailDTO.Email} no existe.");
            throw new KeyNotFoundException("El usuario no existe.");
        }
        if (user.EmailConfirmed)
        {
            Log.Warning(
                $"El usuario con el correo {verifyEmailDTO.Email} ya ha verificado su correo electrónico."
            );
            throw new InvalidOperationException("El correo electrónico ya ha sido verificado.");
        }

        CodeType codeType = CodeType.EmailVerification;
        VerificationCode? verificationCode =
            await _verificationCodeRepository.GetLatestByUserIdAsync(user.Id, codeType);
        if (verificationCode == null)
        {
            Log.Warning(
                $"No se encontró un código de verificación para el usuario: {verifyEmailDTO.Email}"
            );
            throw new KeyNotFoundException("El código de verificación no existe.");
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
                $"Código de verificación incorrecto o expirado para el usuario: {verifyEmailDTO.Email}. Intentos actuales: {attempsCountUpdated}"
            );

            if (attempsCountUpdated >= 5)
            {
                Log.Warning(
                    $"Se ha alcanzado el límite de intentos para el usuario: {verifyEmailDTO.Email}"
                );
                if (await _verificationCodeRepository.DeleteByUserIdAsync(user.Id, codeType))
                {
                    Log.Warning(
                        $"Se ha eliminado el código de verificación para el usuario: {verifyEmailDTO.Email}"
                    );
                    if (await _userRepository.DeleteAsync(user.Id))
                    {
                        Log.Warning($"Se ha eliminado el usuario: {verifyEmailDTO.Email}");
                        throw new ArgumentException(
                            "Se ha alcanzado el límite de intentos. El usuario ha sido eliminado."
                        );
                    }
                }
            }

            if (DateTime.UtcNow >= verificationCode.ExpiryDate)
            {
                Log.Warning(
                    $"El código de verificación ha expirado para el usuario: {verifyEmailDTO.Email}"
                );
                throw new ArgumentException("El código de verificación ha expirado.");
            }
            else
            {
                Log.Warning(
                    $"El código de verificación es incorrecto para el usuario: {verifyEmailDTO.Email}"
                );
                throw new ArgumentException(
                    $"El código de verificación es incorrecto, quedan {5 - attempsCountUpdated} intentos."
                );
            }
        }

        if (await _userRepository.ConfirmEmailAsync(user.Email!))
        {
            if (await _verificationCodeRepository.DeleteByUserIdAsync(user.Id, codeType))
            {
                Log.Warning(
                    $"Se ha eliminado el código de verificación para el usuario: {verifyEmailDTO.Email}"
                );
                await _emailService.SendWelcomeEmailAsync(user.Email!);
                Log.Information(
                    $"El correo electrónico del usuario {verifyEmailDTO.Email} ha sido confirmado exitosamente."
                );
                return "!Ya puedes iniciar sesión y disfrutar de todos los beneficios de Tienda UCN!";
            }
            throw new Exception("Error al confirmar el correo electrónico.");
        }
        throw new Exception("Error al verificar el correo electrónico.");
    }

    /// <summary>
    /// Elimina usuarios no confirmados.
    /// </summary>
    public async Task<int> DeleteUnconfirmedAsync()
    {
        return await _userRepository.DeleteUnconfirmedAsync();
    }

    // En UserService.cs

    public async Task<string> ForgotPasswordAsync(ForgotPasswordDTO forgotPasswordDTO)
    {
        var user = await _userManager.FindByEmailAsync(forgotPasswordDTO.Email);
        if (user == null || !user.EmailConfirmed)
        {
            // No reveles que el usuario no existe para mayor seguridad.
            return "Si existe una cuenta asociada a este correo, se ha enviado un código para restablecer la contraseña.";
        }

        // Genera, guarda y envía un código de 6 dígitos.
        string code = new Random().Next(100000, 999999).ToString();
        var verificationCode = new VerificationCode
        {
            UserId = user.Id,
            Code = code,
            CodeType = CodeType.PasswordReset, // Importante usar el tipo correcto
            ExpiryDate = DateTime.UtcNow.AddMinutes(_verificationCodeExpirationTimeInMinutes),
        };

        await _verificationCodeRepository.CreateAsync(verificationCode);
        Log.Information("Código de reseteo de contraseña generado para {Email}", user.Email);

        // Llama al nuevo método del servicio de email
        await _emailService.SendPasswordResetCodeEmailAsync(user.Email!, user.FirstName, code);

        return "Si existe una cuenta asociada a este correo, se ha enviado un código para restablecer la contraseña.";
    }

    public async Task<string> ResetPasswordAsync(ResetPasswordDTO resetPasswordDTO)
    {
        var user = await _userManager.FindByEmailAsync(resetPasswordDTO.Email);
        if (user == null)
        {
            // No reveles que el usuario no existe.
            return "La contraseña ha sido restablecida exitosamente.";
        }

        // Busca y valida el código de 6 dígitos
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
                "Intento de restablecer contraseña con código inválido o expirado para {Email}",
                resetPasswordDTO.Email
            );
            throw new ArgumentException(
                "El código de restablecimiento es incorrecto o ha expirado."
            );
        }

        // Si el código es válido, se procede a cambiar la contraseña.
        // UserManager requiere un token de reseteo, así que lo generamos internamente justo antes de usarlo.
        var internalToken = await _userManager.GeneratePasswordResetTokenAsync(user);
        var result = await _userManager.ResetPasswordAsync(
            user,
            internalToken,
            resetPasswordDTO.NewPassword
        );

        if (result.Succeeded)
        {
            // Es una buena práctica eliminar el código una vez que se ha utilizado.
            await _verificationCodeRepository.DeleteByUserIdAsync(user.Id, CodeType.PasswordReset);
            Log.Information(
                "Contraseña restablecida exitosamente para {Email}",
                resetPasswordDTO.Email
            );
            return "La contraseña ha sido restablecida exitosamente.";
        }

        // Si falla (ej. la nueva contraseña no cumple las políticas de Identity), lanza los errores.
        var errorMessages = string.Join(" ", result.Errors.Select(e => e.Description));
        Log.Warning(
            "Falló el restablecimiento de contraseña para {Email}: {Errors}",
            resetPasswordDTO.Email,
            errorMessages
        );
        throw new ArgumentException(errorMessages);
    }
}