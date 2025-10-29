public class AuditRecord
{
    public int Id { get; set; }
    public int ChangedById { get; set; }
    public int TargetUserId { get; set; }
    public string Action { get; set; } = default!; // ex: "ChangeStatus", "ChangeRole"
    public string PreviousValue { get; set; } = default!;
    public string NewValue { get; set; } = default!;
    public string? Reason { get; set; }
    public DateTime ChangedAt { get; set; } = DateTime.UtcNow;
} 