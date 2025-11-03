/// <summary>
/// Data Transfer Object for updating a user's role.
/// </summary>
public class UpdateUserRoleDto
{
    /// <summary>
    /// New role for the user. Can be "Cliente" (Customer) or "Administrador" (Administrator).
    /// </summary>
    public string Role { get; set; } = default!;
}
