using TiendaUcnApi.src.Domain.Models;

namespace TiendaUcnApi.src.Application.Services.Interfaces;

/// <summary>
/// Service interface for JWT token generation and management.
/// </summary>
public interface ITokenService
{
    /// <summary>
    /// Generates a JWT authentication token for the specified user.
    /// </summary>
    /// <param name="user">The user for whom to generate the token.</param>
    /// <param name="roleName">The user's role name to include in claims.</param>
    /// <param name="rememberMe">Indicates whether to extend token expiration time.</param>
    /// <returns>A JWT token string containing user claims and authentication information.</returns>
    string GenerateToken(User user, string roleName, bool rememberMe);
}
