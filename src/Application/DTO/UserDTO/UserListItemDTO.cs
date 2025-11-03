/// <summary>
/// Enum representing user account status.
/// </summary>
public enum UserStatusDto
{
    /// <summary>
    /// User account is active and can access the system.
    /// </summary>
    Active,

    /// <summary>
    /// User account is blocked and cannot access the system.
    /// </summary>
    Blocked,
}

/// <summary>
/// Data Transfer Object representing a user in a list view.
/// </summary>
public class UserListItemDto
{
    /// <summary>
    /// User's unique identifier.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// User's first name.
    /// </summary>
    public string FirstName { get; set; } = default!;

    /// <summary>
    /// User's last name.
    /// </summary>
    public string LastName { get; set; } = default!;

    /// <summary>
    /// User's email address.
    /// </summary>
    public string Email { get; set; } = default!;

    /// <summary>
    /// User's role in the system.
    /// </summary>
    public string Role { get; set; } = default!;

    /// <summary>
    /// User's account status (Active or Blocked).
    /// </summary>
    public UserStatusDto Status { get; set; }

    /// <summary>
    /// Date and time when the user account was created.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Date and time of the user's last login (nullable).
    /// </summary>
    public DateTime? LastLogin { get; set; }
}
