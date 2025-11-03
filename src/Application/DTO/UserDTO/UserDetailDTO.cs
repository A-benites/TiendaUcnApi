/// <summary>
/// Data Transfer Object representing detailed user information, extending UserListItemDto.
/// </summary>
public class UserDetailDto : UserListItemDto
{
    /// <summary>
    /// Date and time when the user account was last updated.
    /// </summary>
    public DateTime UpdatedAt { get; set; }
}
