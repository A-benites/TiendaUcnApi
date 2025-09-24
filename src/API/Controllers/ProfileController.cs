using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TiendaUcnApi.src.Application.DTO;
using TiendaUcnApi.src.Application.DTO.AuthDTO;
using TiendaUcnApi.src.Application.Services.Interfaces;

namespace TiendaUcnApi.src.API.Controllers;

[ApiController]
[Route("api/user")]
[Authorize]
public class ProfileController : BaseController
{
    private readonly IProfileService _profileService;

    public ProfileController(IProfileService profileService)
    {
        _profileService = profileService;
    }

    private int GetUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId))
        {
            throw new UnauthorizedAccessException(
                "No se pudo obtener el identificador de usuario del token."
            );
        }
        return userId;
    }

    [HttpGet("profile")]
    public async Task<ActionResult<ProfileDTO>> GetProfile()
    {
        var profile = await _profileService.GetProfileAsync(GetUserId());
        return Ok(new GenericResponse<ProfileDTO>("Perfil obtenido exitosamente", profile));
    }

    [HttpPut("profile")] // MÉTODO Y RUTA CORREGIDOS
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileDTO dto)
    {
        var result = await _profileService.UpdateProfileAsync(GetUserId(), dto);
        return Ok(new GenericResponse<string>(result));
    }

    [HttpPatch("change-password")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDTO dto)
    {
        await _profileService.ChangePasswordAsync(GetUserId(), dto);
        return Ok(new GenericResponse<string>("Contraseña cambiada exitosamente"));
    }

    [HttpPost("verify-email-change")]
    public async Task<IActionResult> VerifyEmailChange([FromBody] VerifyEmailChangeDTO dto)
    {
        var message = await _profileService.VerifyEmailChangeAsync(GetUserId(), dto);
        return Ok(new GenericResponse<string>(message));
    }
}