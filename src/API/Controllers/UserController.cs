using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TiendaUcnApi.src.Domain.Models;
using TiendaUcnApi.src.Infrastructure.Data;

namespace TiendaUcnApi.src.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<IdentityRole<int>> _roleManager;
    private readonly RoleManager<Role> _roleManagerR;

    public UserController(UserManager<User> userManager, RoleManager<Role> roleManager)
    {
        _userManager = userManager;
        _roleManagerR = roleManager;
    }


    [HttpGet("listado")]
    public async Task<ActionResult<IEnumerable<object>>> GetUsuarios([FromQuery] string? search)
    {
        var usersQuery = _userManager.Users.AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            usersQuery = usersQuery.Where(u =>
                u.FirstName.Contains(search) ||
                u.LastName.Contains(search) ||
                u.Email.Contains(search) ||
                u.Rut.Contains(search));
        }

        var users = await usersQuery
            .Select(u => new
            {
                u.Id,
                u.FirstName,
                u.LastName,
                u.Email,
                u.Rut,
                u.Gender,
                u.RegisteredAt,
                u.UpdatedAt,
                IsLocked = u.LockoutEnd != null && u.LockoutEnd > DateTimeOffset.UtcNow
            })
            .ToListAsync();

        return Ok(users);
    }


    [HttpPut("bloquear/{id}")]
    public async Task<IActionResult> BloquearUsuario(int id)
    {
        var user = await _userManager.FindByIdAsync(id.ToString());
        if (user == null)
            return NotFound(new { mensaje = "Usuario no encontrado." });

        bool estaBloqueado = user.LockoutEnd != null && user.LockoutEnd > DateTimeOffset.UtcNow;

        if (estaBloqueado)
        {
            // Desbloquear
            user.LockoutEnd = null;
            await _userManager.UpdateAsync(user);
            return Ok(new { mensaje = $"El usuario {user.Email} ha sido desbloqueado." });
        }
        else
        {
            // Bloquear
            user.LockoutEnd = DateTimeOffset.UtcNow.AddYears(100);
            await _userManager.UpdateAsync(user);
            return Ok(new { mensaje = $"El usuario {user.Email} ha sido bloqueado." });
        }
    }

    [HttpPut("rol/{id}")]
    public async Task<IActionResult> CambiarRol(int id, [FromBody] string nuevoRol)
    {
        var user = await _userManager.FindByIdAsync(id.ToString());
        if (user == null)
            return NotFound(new { mensaje = "Usuario no encontrado." });

        if (!await _roleManagerR.RoleExistsAsync(nuevoRol))
            return BadRequest(new { mensaje = $"El rol '{nuevoRol}' no existe." });

        var rolesActuales = await _userManager.GetRolesAsync(user);

        // Removemos roles actuales y asignamos el nuevo
        await _userManager.RemoveFromRolesAsync(user, rolesActuales);
        await _userManager.AddToRoleAsync(user, nuevoRol);

        return Ok(new { mensaje = $"El rol del usuario {user.Email} se cambió a '{nuevoRol}'." });
    }
}