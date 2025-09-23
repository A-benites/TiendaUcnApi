using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TiendaUcnApi.src.Application.DTO;
using TiendaUcnApi.src.Application.Services.Interfaces;

[ApiController]
[Route("api/[controller]")]
[Authorize] // Todos los endpoints requieren JWT
public class ProfileController : ControllerBase
{
    private readonly IProfileService _profileService;

    public ProfileController(IProfileService profileService)
    {
        _profileService = profileService;
    }

    private int GetUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null)
            throw new Exception("No se encontró el identificador de usuario en el token.");
        return int.Parse(userIdClaim.Value);

    }

    [HttpGet]
    public async Task<ActionResult<ProfileDTO>> GetProfile()
    {
        var profile = await _profileService.GetProfileAsync(GetUserId());
        return Ok(profile);
    }

    [HttpPatch("update")]
    public async Task<IActionResult> UpdateProfile(UpdateProfileDTO dto)
    {
        var success = await _profileService.UpdateProfileAsync(GetUserId(), dto);
        if (!success) return NotFound();
        return Ok(new { message = "Perfil actualizado exitosamente" });
    }

    [HttpPut("change-password")]
    public async Task<IActionResult> ChangePassword(ChangePasswordDTO dto)
    {
        var success = await _profileService.ChangePasswordAsync(GetUserId(), dto);
        if (!success) return BadRequest("Invalid password");
        return Ok(new { message = "Contraseña cambiada exitosamente" });
    }
}