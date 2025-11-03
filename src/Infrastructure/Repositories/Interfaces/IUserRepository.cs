using TiendaUcnApi.src.Application.DTO;
using TiendaUcnApi.src.Domain.Models;

namespace TiendaUcnApi.src.Infrastructure.Repositories.Interfaces;

/// <summary>
/// Repository interface for user data access operations.
/// Handles user authentication, registration, and account management using ASP.NET Core Identity.
/// </summary>
public interface IUserRepository
{
    /// <summary>
    /// Retrieves a user by their identifier.
    /// </summary>
    /// <param name="id">User identifier.</param>
    /// <returns>User entity or null if not found.</returns>
    Task<User?> GetByIdAsync(int id);

    /// <summary>
    /// Retrieves a user by their email address.
    /// </summary>
    /// <param name="email">User's email address.</param>
    /// <returns>User entity or null if not found.</returns>
    Task<User?> GetByEmailAsync(string email);

    /// <summary>
    /// Checks if a user exists with the specified email address.
    /// </summary>
    /// <param name="email">Email address to check.</param>
    /// <returns>True if user exists, false otherwise.</returns>
    Task<bool> ExistsByEmailAsync(string email);

    /// <summary>
    /// Checks if a user exists with the specified Chilean RUT.
    /// </summary>
    /// <param name="rut">Chilean RUT to check.</param>
    /// <returns>True if user exists, false otherwise.</returns>
    Task<bool> ExistsByRutAsync(string rut);

    /// <summary>
    /// Retrieves a user by their Chilean RUT.
    /// </summary>
    /// <param name="rut">User's Chilean RUT.</param>
    /// <param name="trackChanges">Whether to track changes to the entity.</param>
    /// <returns>User entity or null if not found.</returns>
    Task<User?> GetByRutAsync(string rut, bool trackChanges = false);

    /// <summary>
    /// Creates a new user account with hashed password.
    /// </summary>
    /// <param name="user">User entity to create.</param>
    /// <param name="password">User's password (will be hashed).</param>
    /// <returns>True if creation was successful, false otherwise.</returns>
    Task<bool> CreateAsync(User user, string password);

    /// <summary>
    /// Verifies if the provided password is correct for the user.
    /// </summary>
    /// <param name="user">User entity to verify.</param>
    /// <param name="password">Password to verify.</param>
    /// <returns>True if password is correct, false otherwise.</returns>
    Task<bool> CheckPasswordAsync(User user, string password);

    /// <summary>
    /// Retrieves the role name for the specified user.
    /// </summary>
    /// <param name="user">User entity.</param>
    /// <returns>Role name (e.g., "User", "Admin").</returns>
    Task<string> GetUserRoleAsync(User user);

    /// <summary>
    /// Deletes a user account by identifier.
    /// </summary>
    /// <param name="userId">User identifier to delete.</param>
    /// <returns>True if deletion was successful, false otherwise.</returns>
    Task<bool> DeleteAsync(int userId);

    /// <summary>
    /// Confirms a user's email address.
    /// </summary>
    /// <param name="email">Email address to confirm.</param>
    /// <returns>True if confirmation was successful, false otherwise.</returns>
    Task<bool> ConfirmEmailAsync(string email);

    /// <summary>
    /// Deletes all users who have not confirmed their email within the allowed timeframe.
    /// </summary>
    /// <returns>Number of users deleted.</returns>
    Task<int> DeleteUnconfirmedAsync();

    /// <summary>
    /// Retrieves all users who have not confirmed their email address.
    /// </summary>
    /// <returns>List of unconfirmed users.</returns>
    Task<List<User>> GetUnconfirmedUsersAsync();

    /// <summary>
    /// Retrieves all registered users in the database.
    /// </summary>
    /// <returns>List of all users.</returns>
    Task<List<User>> GetAllAsync();
}
