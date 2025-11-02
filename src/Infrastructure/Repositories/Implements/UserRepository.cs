using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;
using TiendaUcnApi.src.Domain.Models;
using TiendaUcnApi.src.Infrastructure.Data;
using TiendaUcnApi.src.Infrastructure.Repositories.Interfaces;

namespace TiendaUcnApi.src.Infrastructure.Repositories.Implements;

/// <summary>
/// Implementation of the user repository.
/// Handles user database operations including authentication, registration, and account management.
/// </summary>
public class UserRepository : IUserRepository
{
    /// <summary>
    /// Application database context.
    /// </summary>
    private readonly AppDbContext _context;

    /// <summary>
    /// ASP.NET Core Identity user manager.
    /// </summary>
    private readonly UserManager<User> _userManager;

    /// <summary>
    /// Days threshold for deleting unconfirmed users.
    /// </summary>
    private readonly int _daysOfDeleteUnconfirmedUsers;

    /// <summary>
    /// Verification code repository.
    /// </summary>
    private readonly IVerificationCodeRepository _verificationCodeRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="UserRepository"/> class.
    /// </summary>
    /// <param name="context">Database context.</param>
    /// <param name="userManager">User manager.</param>
    /// <param name="configuration">Application configuration.</param>
    /// <param name="verificationCodeRepository">Verification code repository.</param>
    public UserRepository(
        AppDbContext context,
        UserManager<User> userManager,
        IConfiguration configuration,
        IVerificationCodeRepository verificationCodeRepository
    )
    {
        _context = context;
        _userManager = userManager;
        _verificationCodeRepository = verificationCodeRepository;
        _daysOfDeleteUnconfirmedUsers =
            configuration.GetValue<int?>("Jobs:DaysOfDeleteUnconfirmedUsers")
            ?? throw new InvalidOperationException(
                "The 'Jobs:DaysOfDeleteUnconfirmedUsers' configuration is not defined."
            );
    }

    /// <summary>
    /// Verifies if the provided password is correct for the user.
    /// </summary>
    /// <param name="user">User to verify.</param>
    /// <param name="password">Password to check.</param>
    /// <returns>True if the password is correct, false otherwise.</returns>
    public async Task<bool> CheckPasswordAsync(User user, string password)
    {
        return await _userManager.CheckPasswordAsync(user, password);
    }

    /// <summary>
    /// Confirms the user's email address.
    /// </summary>
    /// <param name="email">User's email address.</param>
    /// <returns>True if confirmed successfully, false otherwise.</returns>
    public async Task<bool> ConfirmEmailAsync(string email)
    {
        var result = await _context
            .Users.Where(u => u.Email == email)
            .ExecuteUpdateAsync(u => u.SetProperty(x => x.EmailConfirmed, true));
        return result > 0;
    }

    /// <summary>
    /// Creates a new user in the database transactionally.
    /// Automatically assigns the "Cliente" role to the new user.
    /// </summary>
    /// <param name="user">User to create.</param>
    /// <param name="password">User's password.</param>
    /// <returns>True if created successfully, false if there was an error.</returns>
    public async Task<bool> CreateAsync(User user, string password)
    {
        await using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            var userResult = await _userManager.CreateAsync(user, password);
            if (!userResult.Succeeded)
            {
                await transaction.RollbackAsync();
                return false;
            }

            var roleResult = await _userManager.AddToRoleAsync(user, "Cliente");
            if (!roleResult.Succeeded)
            {
                await transaction.RollbackAsync();
                return false;
            }

            await transaction.CommitAsync();
            return true;
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            Log.Error(ex, "An exception occurred during user creation transaction.");
            return false;
        }
    }

    /// <summary>
    /// Deletes a user by their ID.
    /// Also removes all associated data: carts, cart items, verification codes, claims, logins, tokens, and roles.
    /// </summary>
    /// <param name="userId">ID of the user to delete.</param>
    /// <returns>True if deleted successfully, false otherwise.</returns>
    public async Task<bool> DeleteAsync(int userId)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null)
            return false;

        var carts = await _context
            .Carts.Include(c => c.CartItems)
            .Where(c => c.UserId == userId)
            .ToListAsync();

        foreach (var cart in carts)
        {
            _context.CartItems.RemoveRange(cart.CartItems);
            _context.Carts.Remove(cart);
        }

        await _context.SaveChangesAsync();

        await _verificationCodeRepository.DeleteByUserIdAsync(userId);

        var claims = await _userManager.GetClaimsAsync(user);
        if (claims.Any())
            await _userManager.RemoveClaimsAsync(user, claims);

        var logins = await _userManager.GetLoginsAsync(user);
        foreach (var login in logins)
            await _userManager.RemoveLoginAsync(user, login.LoginProvider, login.ProviderKey);

        var tokens = await _context
            .Set<IdentityUserToken<int>>()
            .Where(t => t.UserId == userId)
            .ToListAsync();
        _context.RemoveRange(tokens);

        var roles = await _userManager.GetRolesAsync(user);
        if (roles.Any())
            await _userManager.RemoveFromRolesAsync(user, roles);

        await _context.SaveChangesAsync();

        var result = await _userManager.DeleteAsync(user);

        return result.Succeeded;
    }

    /// <summary>
    /// Deletes unconfirmed users and their associated verification codes.
    /// Only deletes users who registered more than the configured threshold days ago.
    /// </summary>
    /// <returns>Number of users deleted.</returns>
    public async Task<int> DeleteUnconfirmedAsync()
    {
        Log.Information("Starting deletion of unconfirmed users");

        var cutoffDate = DateTime.UtcNow.AddDays(-_daysOfDeleteUnconfirmedUsers);

        var unconfirmedUsers = await _context
            .Users.Where(u => !u.EmailConfirmed && u.RegisteredAt < cutoffDate)
            .Include(u => u.VerificationCodes)
            .ToListAsync();

        if (!unconfirmedUsers.Any())
        {
            Log.Information("No unconfirmed users found to delete");
            return 0;
        }

        foreach (var user in unconfirmedUsers)
        {
            if (user.VerificationCodes.Any())
            {
                await _verificationCodeRepository.DeleteByUserIdAsync(user.Id);
            }
        }

        _context.Users.RemoveRange(unconfirmedUsers);
        await _context.SaveChangesAsync();

        Log.Information($"Deleted {unconfirmedUsers.Count} unconfirmed users");
        return unconfirmedUsers.Count;
    }

    /// <summary>
    /// Checks if a user exists by their email address.
    /// </summary>
    /// <param name="email">Email address to search for.</param>
    /// <returns>True if exists, false otherwise.</returns>
    public async Task<bool> ExistsByEmailAsync(string email)
    {
        return await _context.Users.AnyAsync(u => u.Email == email);
    }

    /// <summary>
    /// Checks if a user exists by their RUT (Chilean national ID).
    /// </summary>
    /// <param name="rut">RUT to search for.</param>
    /// <returns>True if exists, false otherwise.</returns>
    public async Task<bool> ExistsByRutAsync(string rut)
    {
        return await _context.Users.AnyAsync(u => u.Rut == rut);
    }

    /// <summary>
    /// Retrieves a user by their email address.
    /// </summary>
    /// <param name="email">User's email address.</param>
    /// <returns>User found or null if doesn't exist.</returns>
    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _userManager.FindByEmailAsync(email);
    }

    /// <summary>
    /// Retrieves a user by their ID.
    /// </summary>
    /// <param name="id">User ID.</param>
    /// <returns>User found or null if doesn't exist.</returns>
    public async Task<User?> GetByIdAsync(int id)
    {
        return await _userManager.FindByIdAsync(id.ToString());
    }

    /// <summary>
    /// Retrieves a user by their RUT (Chilean national ID).
    /// </summary>
    /// <param name="rut">User's RUT.</param>
    /// <param name="trackChanges">Indicates whether to track changes for updates.</param>
    /// <returns>User found or null if doesn't exist.</returns>
    public async Task<User?> GetByRutAsync(string rut, bool trackChanges = false)
    {
        if (trackChanges)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Rut == rut);
        }

        return await _context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Rut == rut);
    }

    /// <summary>
    /// Retrieves the user's role.
    /// Returns the first role if the user has multiple roles.
    /// </summary>
    /// <param name="user">User to query.</param>
    /// <returns>User's role name.</returns>
    public async Task<string> GetUserRoleAsync(User user)
    {
        var roles = await _userManager.GetRolesAsync(user);
        return roles.FirstOrDefault()!;
    }

    /// <summary>
    /// Retrieves all unconfirmed users.
    /// </summary>
    /// <returns>List of users with unconfirmed emails.</returns>
    public async Task<List<User>> GetUnconfirmedUsersAsync()
    {
        return await _context.Users.Where(u => !u.EmailConfirmed).ToListAsync();
    }

    /// <summary>
    /// Retrieves all registered users.
    /// </summary>
    /// <returns>List of all existing users.</returns>
    public async Task<List<User>> GetAllAsync()
    {
        return await _context.Users.ToListAsync();
    }
}
