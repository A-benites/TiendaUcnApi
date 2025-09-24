using Microsoft.AspNetCore.Mvc;
using Serilog;
using TiendaUcnApi.src.Application.DTO;
using TiendaUcnApi.src.Application.DTO.AuthDTO;
using TiendaUcnApi.src.Application.Services.Interfaces;

namespace TiendaUcnApi.src.API.Controllers;

public class AuthController(IUserService userService) : BaseController
{
    private readonly IUserService _userService = userService;

    /// <summary>
    /// Inicia sesión con el usuario proporcionado.
    /// </summary>
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDTO loginDTO)
    {
        var (token, userId) = await _userService.LoginAsync(loginDTO, HttpContext);
        var buyerId = HttpContext.Items["BuyerId"]?.ToString();
        if (!string.IsNullOrEmpty(buyerId))
        {
            Log.Information(
                "Carrito asociado al usuario. BuyerId: {BuyerId}, UserId: {UserId}",
                buyerId,
                userId
            );
        }
        return Ok(new GenericResponse<string>("Inicio de sesión exitoso", token));
    }

    /// <summary>
    /// Registra un nuevo usuario.
    /// </summary>
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDTO registerDTO)
    {
        var message = await _userService.RegisterAsync(registerDTO, HttpContext);
        return Created(
            "/api/user/profile",
            new GenericResponse<string>("Registro exitoso", message)
        );
    }

    /// <summary>
    /// Verifica el correo electrónico del usuario.
    /// </summary>
    [HttpPost("verify")]
    public async Task<IActionResult> VerifyEmail([FromBody] VerifyEmailDTO verifyEmailDTO)
    {
        var message = await _userService.VerifyEmailAsync(verifyEmailDTO);
        return Ok(
            new GenericResponse<string>("Verificación de correo electrónico exitosa", message)
        );
    }

    /// <summary>
    /// Reenvía el código de verificación al correo electrónico del usuario.
    /// </summary>
    [HttpPost("resend-email-verification-code")]
    public async Task<IActionResult> ResendEmailVerificationCode(
        [FromBody] ResendEmailVerificationCodeDTO resendEmailVerificationCodeDTO
    )
    {
        var message = await _userService.ResendEmailVerificationCodeAsync(
            resendEmailVerificationCodeDTO
        );
        return Ok(
            new GenericResponse<string>("Código de verificación reenviado exitosamente", message)
        );
    }

    /// <summary>
    /// Envía un código para restablecer la contraseña.
    /// </summary>
    [HttpPost("recover-password")]
    public async Task<IActionResult> RecoverPassword([FromBody] ForgotPasswordDTO forgotPasswordDTO)
    {
        var message = await _userService.ForgotPasswordAsync(forgotPasswordDTO);
        return Ok(new GenericResponse<string>("Solicitud de restablecimiento enviada", message));
    }

    /// <summary>
    /// Restablece la contraseña del usuario usando un código.
    /// </summary>
    [HttpPatch("reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDTO resetPasswordDTO)
    {
        var message = await _userService.ResetPasswordAsync(resetPasswordDTO);
        return Ok(new GenericResponse<string>("Contraseña restablecida", message));
    }
}