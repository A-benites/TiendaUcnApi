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
/// Servicio para la gestión del perfil de usuario.
/// </summary>
public class ProfileService : IProfileService
{
    /// <summary>
    /// Repositorio de usuarios.
    /// </summary>
    private readonly IUserRepository _userRepository;

    /// <summary>
    /// Administrador de usuarios de Identity.
    /// </summary>
    private readonly UserManager<User> _userManager;

    /// <summary>
    /// Servicio para envío de correos electrónicos.
    /// </summary>
    private readonly IEmailService _emailService;

    /// <summary>
    /// Repositorio de códigos de verificación.
    /// </summary>
    private readonly IVerificationCodeRepository _verificationCodeRepository;

    /// <summary>
    /// Configuración de la aplicación.
    /// </summary>
    private readonly IConfiguration _configuration;

    /// <summary>
    /// Constructor con todas las dependencias necesarias.
    /// </summary>
    /// <param name="userRepository">Repositorio de usuarios.</param>
    /// <param name="userManager">Administrador de usuarios.</param>
    /// <param name="emailService">Servicio de email.</param>
    /// <param name="verificationCodeRepository">Repositorio de códigos de verificación.</param>
    /// <param name="configuration">Configuración de la aplicación.</param>
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
    /// Obtiene el perfil del usuario por su ID.
    /// </summary>
    /// <param name="userId">ID del usuario.</param>
    /// <returns>DTO con los datos del perfil.</returns>
    public async Task<ProfileDTO> GetProfileAsync(int userId)
    {
        var user =
            await _userRepository.GetByIdAsync(userId)
            ?? throw new KeyNotFoundException("Usuario no encontrado.");
        return user.Adapt<ProfileDTO>();
    }

    /// <summary>
    /// Actualiza los datos del perfil del usuario.
    /// </summary>
    /// <param name="userId">ID del usuario.</param>
    /// <param name="dto">DTO con los datos a actualizar.</param>
    /// <returns>Mensaje de confirmación.</returns>
    public async Task<string> UpdateProfileAsync(int userId, UpdateProfileDTO dto)
    {
        var user =
            await _userRepository.GetByIdAsync(userId)
            ?? throw new KeyNotFoundException("Usuario no encontrado.");
        var changesMade = false;

        if (
            !string.IsNullOrEmpty(dto.Email)
            && !dto.Email.Equals(user.Email, StringComparison.OrdinalIgnoreCase)
        )
        {
            if (await _userRepository.ExistsByEmailAsync(dto.Email))
            {
                throw new ArgumentException(
                    "El correo electrónico ya está en uso por otro usuario."
                );
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
                "Usuario {UserId} solicitó cambio de email a {NewEmail}. Se envió código de verificación.",
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
            Log.Information(
                "Perfil del usuario {UserId} actualizado (datos no relacionados al email).",
                userId
            );
        }

        return "Tus datos han sido actualizados. Si solicitaste un cambio de correo, por favor verifica el código enviado a tu nueva dirección.";
    }

    /// <summary>
    /// Verifica el cambio de correo electrónico del usuario.
    /// </summary>
    /// <param name="userId">ID del usuario.</param>
    /// <param name="dto">DTO con el nuevo email y código de verificación.</param>
    /// <returns>Mensaje de confirmación.</returns>
    public async Task<string> VerifyEmailChangeAsync(int userId, VerifyEmailChangeDTO dto)
    {
        var user =
            await _userRepository.GetByIdAsync(userId)
            ?? throw new KeyNotFoundException("Usuario no encontrado.");
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
            throw new ArgumentException("El código de verificación es incorrecto o ha expirado.");
        }

        user.Email = dto.NewEmail;
        user.UserName = dto.NewEmail;
        user.NormalizedEmail = _userManager.KeyNormalizer.NormalizeEmail(dto.NewEmail);
        user.NormalizedUserName = _userManager.KeyNormalizer.NormalizeName(dto.NewEmail);
        user.EmailConfirmed = true;

        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
        {
            throw new Exception("Ocurrió un error al actualizar tu correo electrónico.");
        }

        await _verificationCodeRepository.DeleteByUserIdAsync(user.Id, CodeType.EmailChange);
        Log.Information(
            "El usuario {UserId} ha confirmado exitosamente su nuevo email: {NewEmail}",
            userId,
            dto.NewEmail
        );
        return "Tu correo electrónico ha sido actualizado exitosamente.";
    }

    /// <summary>
    /// Cambia la contraseña del usuario.
    /// </summary>
    /// <param name="userId">ID del usuario.</param>
    /// <param name="dto">DTO con la contraseña actual y la nueva.</param>
    public async Task ChangePasswordAsync(int userId, ChangePasswordDTO dto)
    {
        var user =
            await _userRepository.GetByIdAsync(userId)
            ?? throw new KeyNotFoundException("Usuario no encontrado.");
        var result = await _userManager.ChangePasswordAsync(user, dto.OldPassword, dto.NewPassword);

        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            Log.Warning(
                "Falló el cambio de contraseña para el usuario {UserId}: {Errors}",
                userId,
                errors
            );
            throw new ArgumentException($"Error al cambiar la contraseña: {errors}");
        }

        await _userManager.UpdateSecurityStampAsync(user);

        Log.Information(
            "Contraseña del usuario {UserId} cambiada exitosamente y sesiones invalidadas.",
            userId
        );
    }
}