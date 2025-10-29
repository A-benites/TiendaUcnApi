using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TiendaUcnApi.src.Application.Services.Implements;
using TiendaUcnApi.src.Application.Services.Interfaces;
using TiendaUcnApi.src.Domain.Models;

[ApiController]
[Route("api/admin/users")]
[Authorize(Roles = "Administrador")]
public class UserController : ControllerBase
{
    private readonly IUserAdminService _svc;
    private readonly ILogger<UserController> _logger;
    private readonly UserManager<User> _userManager;

    public UserController(IUserAdminService svc, ILogger<UserController> logger, UserManager<User> userManager)
    {
        _svc = svc;
        _logger = logger;
        _userManager = userManager;
    }

    /// <summary>
    /// GET /api/admin/users
    /// List users with pagination, filters and safe ordering.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetUsers(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? role = null,
        [FromQuery] string? status = null,
        [FromQuery] string? email = null,
        [FromQuery] DateTime? createdFrom = null,
        [FromQuery] DateTime? createdTo = null,
        [FromQuery] string? orderBy = null,
        [FromQuery] string? orderDir = "desc",
        CancellationToken ct = default)
    {
        try
        {
            var result = await _svc.GetUsersAsync(page, pageSize, role, status, email, createdFrom, createdTo, orderBy, orderDir, ct);
            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Invalid query for GetUsers");
            return BadRequest(new { code = 400, message = ex.Message });
        }
    }

    /// <summary>
    /// GET /api/admin/users/{id}
    /// Get user detail (DTO).
    /// </summary>
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetUser(int id, CancellationToken ct = default)
    {
        var user = await _svc.GetUserByIdAsync(id, ct);
        if (user == null) return NotFound(new { code = 404, message = "User not found" });
        return Ok(user);
    }

    /// <summary>
    /// PATCH /api/admin/users/{id}/status
    /// Change user status (active/blocked). Body: UpdateUserStatusDto
    /// </summary>
    [HttpPatch("{id:int}/status")]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateUserStatusDto dto, CancellationToken ct = default)
    {
        if (dto == null) return BadRequest(new { code = 400, message = "Body required" });

        // validate enum
        if (!Enum.TryParse<UserStatusDto>(dto.Status, true, out _))
            return BadRequest(new { code = 400, message = "Invalid status value" });

        try
        {
            // get admin id from token
            var adminIdStr = User.FindFirst("id")?.Value ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (adminIdStr == null) return Unauthorized(new { code = 401, message = "Invalid token" });

            int adminId = int.Parse(adminIdStr);
            await _svc.ChangeUserStatusAsync(adminId, id, dto, ct);
            return NoContent(); // 204 success (could be 200 with message)
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new { code = 404, message = "User not found" });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { code = 409, message = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { code = 400, message = ex.Message });
        }
    }

    /// <summary>
    /// PATCH /api/admin/users/{id}/role
    /// Change user role. Body: UpdateUserRoleDto
    /// </summary>
    [HttpPatch("{id:int}/role")]
    public async Task<IActionResult> UpdateRole(int id, [FromBody] UpdateUserRoleDto dto, CancellationToken ct = default)
    {
        if (dto == null) return BadRequest(new { code = 400, message = "Body required" });

        try
        {
            var adminIdStr = User.FindFirst("id")?.Value ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (adminIdStr == null) return Unauthorized(new { code = 401, message = "Invalid token" });
            int adminId = int.Parse(adminIdStr);

            await _svc.ChangeUserRoleAsync(adminId, id, dto, ct);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new { code = 404, message = "User not found" });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { code = 409, message = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { code = 400, message = ex.Message });
        }
    }
}
