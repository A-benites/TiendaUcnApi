/// <summary>
/// Represents an audit record for tracking administrative actions on user accounts.
/// Used to maintain a history of status changes, role changes, and other administrative operations.
/// </summary>
public class AuditRecord
{
    /// <summary>
    /// Gets or sets the unique identifier for the audit record.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the ID of the administrator who performed the action.
    /// </summary>
    public int ChangedById { get; set; }

    /// <summary>
    /// Gets or sets the ID of the user who was affected by the action.
    /// </summary>
    public int TargetUserId { get; set; }

    /// <summary>
    /// Gets or sets the type of action performed.
    /// Examples: "ChangeStatus", "ChangeRole", "UpdateProfile", etc.
    /// </summary>
    public string Action { get; set; } = default!;

    /// <summary>
    /// Gets or sets the value before the change was made.
    /// </summary>
    public string PreviousValue { get; set; } = default!;

    /// <summary>
    /// Gets or sets the new value after the change was made.
    /// </summary>
    public string NewValue { get; set; } = default!;

    /// <summary>
    /// Gets or sets the optional reason or justification for the change.
    /// </summary>
    public string? Reason { get; set; }

    /// <summary>
    /// Gets or sets the UTC timestamp when the change was made.
    /// </summary>
    public DateTime ChangedAt { get; set; } = DateTime.UtcNow;
}
