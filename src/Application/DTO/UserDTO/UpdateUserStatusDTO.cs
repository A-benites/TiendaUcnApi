/// <summary>
/// Data Transfer Object for updating a user's account status.
/// </summary>
public class UpdateUserStatusDto
{
    /// <summary>
    /// New status for the user. Can be "active" or "blocked".
    /// </summary>
    public string Status { get; set; } = default!;

    /// <summary>
    /// Optional reason for the status change (e.g., reason for blocking).
    /// </summary>
    public string? Reason { get; set; }
}
