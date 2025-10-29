
public interface IUserAdminService
{
    Task<PagedResult<UserListItemDto>> GetUsersAsync(int page, int pageSize,
        string? role, string? status, string? email, DateTime? createdFrom, DateTime? createdTo,
        string? orderBy, string? orderDir, CancellationToken ct = default);

    Task<UserDetailDto?> GetUserByIdAsync(int id, CancellationToken ct = default);

    Task ChangeUserStatusAsync(int adminId, int targetUserId, UpdateUserStatusDto dto, CancellationToken ct = default);

    Task ChangeUserRoleAsync(int adminId, int targetUserId, UpdateUserRoleDto dto, CancellationToken ct = default);
}