/// <summary>
/// Service interface for user administration operations.
/// Only accessible by administrators for user management.
/// </summary>
public interface IUserAdminService
{
    /// <summary>
    /// Retrieves a paginated list of users with filtering and sorting options.
    /// </summary>
    /// <param name="page">The page number to retrieve (1-indexed).</param>
    /// <param name="pageSize">Number of users per page.</param>
    /// <param name="role">Optional: Filter by role name.</param>
    /// <param name="status">Optional: Filter by account status.</param>
    /// <param name="email">Optional: Filter by email containing this value.</param>
    /// <param name="createdFrom">Optional: Filter by creation date (from).</param>
    /// <param name="createdTo">Optional: Filter by creation date (to).</param>
    /// <param name="orderBy">Optional: Field to sort by.</param>
    /// <param name="orderDir">Optional: Sort direction (asc/desc).</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Paginated result containing user list items.</returns>
    Task<PagedResult<UserListItemDto>> GetUsersAsync(
        int page,
        int pageSize,
        string? role,
        string? status,
        string? email,
        DateTime? createdFrom,
        DateTime? createdTo,
        string? orderBy,
        string? orderDir,
        CancellationToken ct = default
    );

    /// <summary>
    /// Retrieves detailed information for a specific user by ID.
    /// </summary>
    /// <param name="id">The user identifier.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>User details or null if not found.</returns>
    Task<UserDetailDto?> GetUserByIdAsync(int id, CancellationToken ct = default);

    /// <summary>
    /// Changes the active status of a user account (activate/deactivate).
    /// </summary>
    /// <param name="adminId">Administrator identifier performing the change.</param>
    /// <param name="targetUserId">User identifier whose status will be changed.</param>
    /// <param name="dto">DTO containing the new status.</param>
    /// <param name="ct">Cancellation token.</param>
    Task ChangeUserStatusAsync(
        int adminId,
        int targetUserId,
        UpdateUserStatusDto dto,
        CancellationToken ct = default
    );

    /// <summary>
    /// Changes the role of a user account (e.g., User to Admin).
    /// </summary>
    /// <param name="adminId">Administrator identifier performing the change.</param>
    /// <param name="targetUserId">User identifier whose role will be changed.</param>
    /// <param name="dto">DTO containing the new role name.</param>
    /// <param name="ct">Cancellation token.</param>
    Task ChangeUserRoleAsync(
        int adminId,
        int targetUserId,
        UpdateUserRoleDto dto,
        CancellationToken ct = default
    );
}
