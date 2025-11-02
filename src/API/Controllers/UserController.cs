using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TiendaUcnApi.src.Application.Services.Implements;
using TiendaUcnApi.src.Application.Services.Interfaces;
using TiendaUcnApi.src.Domain.Models;

/// <summary>
/// Controller for user administration.
/// Provides user listing, detail retrieval, status management, and role assignment.
/// Only accessible by users with "Administrador" role.
/// </summary>
[ApiController]
[Route("api/admin/users")]
[Authorize(Roles = "Administrador")]
public class UserController : ControllerBase
{
    private readonly IUserAdminService _svc;
    private readonly ILogger<UserController> _logger;
    private readonly UserManager<User> _userManager;

    /// <summary>
    /// Initializes a new instance of the <see cref="UserController"/> class.
    /// </summary>
    /// <param name="svc">The user administration service.</param>
    /// <param name="logger">The logger instance.</param>
    /// <param name="userManager">ASP.NET Core Identity user manager.</param>
    public UserController(
        IUserAdminService svc,
        ILogger<UserController> logger,
        UserManager<User> userManager
    )
    {
        _svc = svc;
        _logger = logger;
        _userManager = userManager;
    }

    /// <summary>
    /// Retrieves a paginated list of users with optional filters and ordering.
    /// Supports filtering by role, status, email, and creation date range.
    /// </summary>
    /// <param name="page">Page number (default: 1).</param>
    /// <param name="pageSize">Items per page (default: 20).</param>
    /// <param name="role">Filter by user role (optional).</param>
    /// <param name="status">Filter by user status (optional).</param>
    /// <param name="email">Filter by email address (optional).</param>
    /// <param name="createdFrom">Filter by creation date from (optional).</param>
    /// <param name="createdTo">Filter by creation date to (optional).</param>
    /// <param name="orderBy">Field to order by (optional).</param>
    /// <param name="orderDir">Order direction: "asc" or "desc" (default: "desc").</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Paginated user list.</returns>
    /// <response code="200">Returns the user list.</response>
    /// <response code="400">Invalid query parameters.</response>
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
        CancellationToken ct = default
    )
    {
        try
        {
            var result = await _svc.GetUsersAsync(
                page,
                pageSize,
                role,
                status,
                email,
                createdFrom,
                createdTo,
                orderBy,
                orderDir,
                ct
            );
            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Invalid query for GetUsers");
            return BadRequest(new { code = 400, message = ex.Message });
        }
    }

    /// <summary>
    /// Retrieves detailed information for a specific user by ID.
    /// </summary>
    /// <param name="id">The user identifier.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>User details.</returns>
    /// <response code="200">Returns the user details.</response>
    /// <response code="404">User not found.</response>
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetUser(int id, CancellationToken ct = default)
    {
        var user = await _svc.GetUserByIdAsync(id, ct);
        if (user == null)
            return NotFound(new { code = 404, message = "User not found" });
        return Ok(user);
    }

    /// <summary>
    /// Updates a user's status (active/blocked).
    /// Requires admin authentication and validates the target user can be modified.
    /// </summary>
    /// <param name="id">The user identifier to update.</param>
    /// <param name="dto">DTO containing the new status value.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>No content on success.</returns>
    /// <response code="204">Status updated successfully.</response>
    /// <response code="400">Invalid status value or body.</response>
    /// <response code="401">Invalid admin token.</response>
    /// <response code="404">User not found.</response>
    /// <response code="409">Cannot modify this user (e.g., trying to block yourself).</response>
    [HttpPatch("{id:int}/status")]
    public async Task<IActionResult> UpdateStatus(
        int id,
        [FromBody] UpdateUserStatusDto dto,
        CancellationToken ct = default
    )
    {
        if (dto == null)
            return BadRequest(new { code = 400, message = "Body required" });

        // validate enum
        if (!Enum.TryParse<UserStatusDto>(dto.Status, true, out _))
            return BadRequest(new { code = 400, message = "Invalid status value" });

        try
        {
            // get admin id from token
            var adminIdStr =
                User.FindFirst("id")?.Value ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (adminIdStr == null)
                return Unauthorized(new { code = 401, message = "Invalid token" });

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
    /// Changes a user's role.
    /// Requires admin authentication and validates the role assignment is allowed.
    /// </summary>
    /// <param name="id">The user identifier to update.</param>
    /// <param name="dto">DTO containing the new role.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>No content on success.</returns>
    /// <response code="204">Role updated successfully.</response>
    /// <response code="400">Invalid role value or body.</response>
    /// <response code="401">Invalid admin token.</response>
    /// <response code="404">User not found.</response>
    /// <response code="409">Cannot change this user's role (e.g., trying to change your own role).</response>
    [HttpPatch("{id:int}/role")]
    public async Task<IActionResult> UpdateRole(
        int id,
        [FromBody] UpdateUserRoleDto dto,
        CancellationToken ct = default
    )
    {
        if (dto == null)
            return BadRequest(new { code = 400, message = "Body required" });

        try
        {
            var adminIdStr =
                User.FindFirst("id")?.Value ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (adminIdStr == null)
                return Unauthorized(new { code = 401, message = "Invalid token" });
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
