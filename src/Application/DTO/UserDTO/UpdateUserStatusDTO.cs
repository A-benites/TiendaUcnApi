public class UpdateUserStatusDto
{
    public string Status { get; set; } = default!; // "active" or "blocked"
    public string? Reason { get; set; }
}