using Microsoft.AspNetCore.Mvc;
using Tienda_UCN_api.src.Application.DTO; // Asegúrate que el namespace sea el correcto
using TiendaUcnApi.src.Application.DTO.AuthDTO; // Asegúrate que el namespace sea el correcto
using TiendaUcnApi.src.Application.Services.Interfaces;

namespace TiendaUcnApi.src.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase // <- SOLUCIÓN 1: Heredar de ControllerBase
{
    private readonly IUserService _userService;

    // El constructor ahora es más simple
    public AuthController(IUserService userService)
    {
        _userService = userService;
    }

    /// <summary>
    /// Registra un nuevo usuario.
    /// </summary>
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDTO registerDTO)
    {
        // SOLUCIÓN 2: Usar la variable de instancia _userService y la propiedad HttpContext
        var message = await _userService.RegisterAsync(registerDTO, HttpContext);
        return Ok(new GenericResponse<string>("Registro exitoso", message));
    }

    /// <summary>
    /// Verifica el correo electrónico del usuario.
    /// </summary>
    [HttpPost("verify-email")]
    public async Task<IActionResult> VerifyEmail([FromBody] VerifyEmailDTO verifyEmailDTO)
    {
        var message = await _userService.VerifyEmailAsync(verifyEmailDTO);
        return Ok(
            new GenericResponse<string>("Verificación de correo electrónico exitosa", message)
        );
    }

    // Puedes descomentar este método cuando lo necesites
    /*
    /// <summary>
    /// Reenvía el código de verificación al correo electrónico del usuario.
    /// </summary>
    [HttpPost("resend-email-verification-code")]
    public async Task<IActionResult> ResendEmailVerificationCode([FromBody] ResendEmailVerificationCodeDTO resendEmailVerificationCodeDTO)
    {
        var message = await _userService.ResendEmailVerificationCodeAsync(resendEmailVerificationCodeDTO);
        return Ok(
            new GenericResponse<string>("Código de verificación reenviado exitosamente", message)
        );
    }
    */
}