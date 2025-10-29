// Application/Services/Implements/UserAdminService.cs
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TiendaUcnApi.src.Infrastructure.Data;
using TiendaUcnApi.src.Domain.Models; // User, Role

public class UserAdminService : IUserAdminService
{
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<Role> _roleManager;
    private readonly AppDbContext _db;
    private readonly ILogger<UserAdminService> _logger;

    // whitelist for ordering
    private static readonly HashSet<string> OrderWhitelist = new(StringComparer.OrdinalIgnoreCase)
    {
        "createdAt", "lastLogin", "email"
    };

    public UserAdminService(
        UserManager<User> userManager,
        RoleManager<Role> roleManager,
        AppDbContext db,
        ILogger<UserAdminService> logger)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _db = db;
        _logger = logger;
    }

    public async Task<PagedResult<UserListItemDto>> GetUsersAsync(int page, int pageSize,
        string? role, string? status, string? email, DateTime? createdFrom, DateTime? createdTo,
        string? orderBy, string? orderDir, CancellationToken ct = default)
    {
        if (page <= 0) page = 1;
        if (pageSize <= 0 || pageSize > 100) pageSize = 20;

        var query = _userManager.Users.AsQueryable();

        // role filter (join AspNetUserRoles)
        if (!string.IsNullOrWhiteSpace(role))
        {
            var roleEntity = await _roleManager.FindByNameAsync(role);
            if (roleEntity == null)
                return new PagedResult<UserListItemDto> { Page = page, PageSize = pageSize, TotalCount = 0 };
            var roleUsers = _db.Set<IdentityUserRole<int>>()
                .Where(ur => ur.RoleId == roleEntity.Id)
                .Select(ur => ur.UserId);
            query = query.Where(u => roleUsers.Contains(u.Id));
        }

        if (!string.IsNullOrWhiteSpace(status))
        {
            if (Enum.TryParse<UserStatusDto>(status, true, out var st))
            {
                if (st == UserStatusDto.Blocked)
                    query = query.Where(u => u.LockoutEnd != null && u.LockoutEnd > DateTimeOffset.UtcNow);
                else
                    query = query.Where(u => u.LockoutEnd == null || u.LockoutEnd <= DateTimeOffset.UtcNow);
            }
            else
            {
                throw new ArgumentException("Invalid status filter");
            }
        }

        if (!string.IsNullOrWhiteSpace(email))
            query = query.Where(u => u.Email.Contains(email));

        if (createdFrom.HasValue)
            query = query.Where(u => u.RegisteredAt >= createdFrom.Value);

        if (createdTo.HasValue)
            query = query.Where(u => u.RegisteredAt <= createdTo.Value);

        // ordering safe
        if (!string.IsNullOrWhiteSpace(orderBy) && OrderWhitelist.Contains(orderBy))
        {
            bool asc = string.Equals(orderDir, "asc", StringComparison.OrdinalIgnoreCase);
            query = orderBy.ToLower() switch
            {
                "createdat" => asc ? query.OrderBy(u => u.RegisteredAt) : query.OrderByDescending(u => u.RegisteredAt),
                "lastlogin" => asc ? query.OrderBy(u => u.LockoutEnd) /*as placeholder*/ : query.OrderByDescending(u => u.LockoutEnd),
                "email" => asc ? query.OrderBy(u => u.Email) : query.OrderByDescending(u => u.Email),
                _ => query.OrderByDescending(u => u.RegisteredAt)
            };
        }
        else
        {
            query = query.OrderByDescending(u => u.RegisteredAt);
        }

        var total = await query.LongCountAsync(ct);
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(u => new UserListItemDto
            {
                Id = u.Id,
                FirstName = u.FirstName,
                LastName = u.LastName,
                Email = u.Email,
                Role = _db.Set<IdentityUserRole<int>>()
                          .Where(ur => ur.UserId == u.Id)
                          .Join(_db.Roles, ur => ur.RoleId, r => r.Id, (ur, r) => r.Name)
                          .FirstOrDefault() ?? string.Empty,
                Status = (u.LockoutEnd != null && u.LockoutEnd > DateTimeOffset.UtcNow) ? UserStatusDto.Blocked : UserStatusDto.Active,
                CreatedAt = u.RegisteredAt,
                LastLogin = u.LockoutEnd.HasValue ? u.LockoutEnd.Value.DateTime : null // example: if you store last login elsewhere adapt accordingly
            })
            .ToListAsync(ct);

        return new PagedResult<UserListItemDto>
        {
            Page = page,
            PageSize = pageSize,
            TotalCount = total,
            Items = items
        };
    }

    public async Task<UserDetailDto?> GetUserByIdAsync(int id, CancellationToken ct = default)
    {
        var u = await _userManager.Users.FirstOrDefaultAsync(x => x.Id == id, ct);
        if (u == null) return null;

        var role = await (from ur in _db.Set<IdentityUserRole<int>>()
                          where ur.UserId == u.Id
                          join r in _db.Roles on ur.RoleId equals r.Id
                          select r.Name).FirstOrDefaultAsync(ct);

        return new UserDetailDto
        {
            Id = u.Id,
            FirstName = u.FirstName,
            LastName = u.LastName,
            Email = u.Email,
            Role = role ?? string.Empty,
            Status = (u.LockoutEnd != null && u.LockoutEnd > DateTimeOffset.UtcNow) ? UserStatusDto.Blocked : UserStatusDto.Active,
            CreatedAt = u.RegisteredAt,
            LastLogin = u.LockoutEnd?.DateTime,
            UpdatedAt = u.UpdatedAt
        };
    }

    public async Task ChangeUserStatusAsync(int adminId, int targetUserId, UpdateUserStatusDto dto, CancellationToken ct = default)
    {
        if (!Enum.TryParse<UserStatusDto>(dto.Status, true, out var newStatus))
            throw new ArgumentException("Invalid status");

        var admin = await _userManager.FindByIdAsync(adminId.ToString());
        var target = await _userManager.FindByIdAsync(targetUserId.ToString());
        if (target == null) throw new KeyNotFoundException("User not found");

        // business rule: admin cannot block themselves
        if (adminId == targetUserId && newStatus == UserStatusDto.Blocked)
            throw new InvalidOperationException("Admin cannot block themselves");

        // check if trying to block last admin
        if (newStatus == UserStatusDto.Blocked)
        {
            var targetRoles = await _userManager.GetRolesAsync(target);
            if (targetRoles.Contains("Administrador"))
            {
                // count admins
                var adminRole = await _roleManager.FindByNameAsync("Administrador");
                if (adminRole == null) throw new InvalidOperationException("Role 'Administrador' missing");

                var adminsCount = await (from ur in _db.Set<IdentityUserRole<int>>()
                                         where ur.RoleId == adminRole.Id
                                         select ur.UserId).Distinct().CountAsync();

                if (adminsCount <= 1)
                    throw new InvalidOperationException("Cannot block the last admin");
            }
        }

        // effect: set LockoutEnd accordingly
        bool isCurrentlyBlocked = target.LockoutEnd != null && target.LockoutEnd > DateTimeOffset.UtcNow;
        if (newStatus == UserStatusDto.Blocked && !isCurrentlyBlocked)
        {
            target.LockoutEnd = DateTimeOffset.UtcNow.AddYears(100);
        }
        else if (newStatus == UserStatusDto.Active && isCurrentlyBlocked)
        {
            target.LockoutEnd = null;
        }
        else
        {
            // no change
            return;
        }

        // Update and audit
        var prev = isCurrentlyBlocked ? "Blocked" : "Active";
        var next = newStatus.ToString();

        await _userManager.UpdateAsync(target);

        // Invalidate sessions: update security stamp -> requires token validation to check stamp or use token version approach
        await _userManager.UpdateSecurityStampAsync(target);

        // save audit record
        var audit = new AuditRecord
        {
            ChangedById = adminId,
            TargetUserId = targetUserId,
            Action = "ChangeStatus",
            PreviousValue = prev,
            NewValue = next,
            Reason = dto.Reason,
            ChangedAt = DateTime.UtcNow
        };
        _db.AuditRecords.Add(audit);
        await _db.SaveChangesAsync(ct);

        _logger.LogInformation("User status changed by admin {AdminId}: target {Target} {Prev} -> {New}", adminId, targetUserId, prev, next);
    }

    public async Task ChangeUserRoleAsync(int adminId, int targetUserId, UpdateUserRoleDto dto, CancellationToken ct = default)
    {
        // validate role exists and allowed
        var roleName = dto.Role;
        if (string.IsNullOrWhiteSpace(roleName))
            throw new ArgumentException("Role is required");

        // restrict enumerated values
        var allowed = new[] { "Cliente", "Administrador" };
        if (!allowed.Contains(roleName))
            throw new ArgumentException("Role not allowed");

        var target = await _userManager.FindByIdAsync(targetUserId.ToString());
        if (target == null) throw new KeyNotFoundException("User not found");

        var roleExists = await _roleManager.RoleExistsAsync(roleName);
        if (!roleExists) throw new ArgumentException("Role does not exist");

        var currentRoles = await _userManager.GetRolesAsync(target);
        var previousRole = currentRoles.FirstOrDefault() ?? string.Empty;

        // business rule: prevent last admin from removing own admin
        if (previousRole == "Administrador" && roleName != "Administrador")
        {
            var adminRole = await _roleManager.FindByNameAsync("Administrador");
            var adminsCount = await (from ur in _db.Set<IdentityUserRole<int>>()
                                     where ur.RoleId == adminRole.Id
                                     select ur.UserId).Distinct().CountAsync();
            if (adminsCount <= 1 && currentRoles.Contains("Administrador"))
                throw new InvalidOperationException("Cannot remove the last admin");
        }

        // proceed: remove all roles and add new one (or smarter: remove only different)
        await _userManager.RemoveFromRolesAsync(target, currentRoles);
        await _userManager.AddToRoleAsync(target, roleName);

        // audit
        var audit = new AuditRecord
        {
            ChangedById = adminId,
            TargetUserId = targetUserId,
            Action = "ChangeRole",
            PreviousValue = previousRole,
            NewValue = roleName,
            ChangedAt = DateTime.UtcNow
        };
        _db.AuditRecords.Add(audit);

        // if role reduces privileges, invalidate sessions
        if (previousRole == "Administrador" && roleName != "Administrador")
        {
            await _userManager.UpdateSecurityStampAsync(target);
        }

        await _db.SaveChangesAsync(ct);
        _logger.LogInformation("User role changed by admin {AdminId}: target {Target} {Prev} -> {New}", adminId, targetUserId, previousRole, roleName);
    }
}
