using Microsoft.AspNetCore.Identity;

namespace TiendaUcnApi.src.Domain.Models;

/// <summary>
/// Represents a user role in the system, extending ASP.NET Core Identity role with integer primary key.
/// </summary>
public class Role : IdentityRole<int>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Role"/> class.
    /// </summary>
    public Role()
        : base() { }
}
