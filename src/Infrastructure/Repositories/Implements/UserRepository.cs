using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;
using TiendaUcnApi.src.Domain.Models;
using TiendaUcnApi.src.Infrastructure.Data;
using TiendaUcnApi.src.Infrastructure.Repositories.Interfaces;

namespace TiendaUcnApi.src.Infrastructure.Repositories.Implements;

/// <summary>
/// Implementación del repositorio de usuarios.
/// </summary>
public class UserRepository : IUserRepository
{
    /// <summary>
    /// Contexto de base de datos de la aplicación.
    /// </summary>
    private readonly AppDbContext _context;

    /// <summary>
    /// Gestor de usuarios de Identity.
    /// </summary>
    private readonly UserManager<User> _userManager;

    /// <summary>
    /// Días para eliminar usuarios no confirmados.
    /// </summary>
    private readonly int _daysOfDeleteUnconfirmedUsers;

    /// <summary>
    /// Repositorio de códigos de verificación.
    /// </summary>
    private readonly IVerificationCodeRepository _verificationCodeRepository;

    /// <summary>
    /// Constructor que inyecta todas las dependencias necesarias.
    /// </summary>
    /// <param name="context">Contexto de base de datos.</param>
    /// <param name="userManager">Gestor de usuarios.</param>
    /// <param name="configuration">Configuración de la aplicación.</param>
    /// <param name="verificationCodeRepository">Repositorio de códigos de verificación.</param>
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
                "La configuración 'Jobs:DaysOfDeleteUnconfirmedUsers' no está definida."
            );
    }

    /// <summary>
    /// Verifica si la contraseña proporcionada es correcta para el usuario.
    /// </summary>
    /// <param name="user">Usuario a verificar.</param>
    /// <param name="password">Contraseña a comprobar.</param>
    /// <returns>True si la contraseña es correcta, false si no.</returns>
    public async Task<bool> CheckPasswordAsync(User user, string password)
    {
        return await _userManager.CheckPasswordAsync(user, password);
    }

    /// <summary>
    /// Confirma el correo electrónico del usuario.
    /// </summary>
    /// <param name="email">Correo electrónico del usuario.</param>
    /// <returns>True si se confirmó correctamente, false si no.</returns>
    public async Task<bool> ConfirmEmailAsync(string email)
    {
        var result = await _context
            .Users.Where(u => u.Email == email)
            .ExecuteUpdateAsync(u => u.SetProperty(x => x.EmailConfirmed, true));
        return result > 0;
    }

    /// <summary>
    /// Crea un nuevo usuario en la base de datos de forma transaccional.
    /// </summary>
    /// <param name="user">Usuario a crear.</param>
    /// <param name="password">Contraseña del usuario.</param>
    /// <returns>True si se creó correctamente, false si hubo error.</returns>
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
            Log.Error(ex, "Ocurrió una excepción durante la transacción de creación de usuario.");
            return false;
        }
    }

    /// <summary>
    /// Elimina un usuario por su ID.
    /// </summary>
    /// <param name="userId">ID del usuario a eliminar.</param>
    /// <returns>True si se eliminó correctamente, false si no.</returns>
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
    /// Elimina usuarios no confirmados y sus códigos de verificación asociados.
    /// </summary>
    /// <returns>Cantidad de usuarios eliminados.</returns>
    public async Task<int> DeleteUnconfirmedAsync()
    {
        Log.Information("Iniciando eliminación de usuarios no confirmados");

        var cutoffDate = DateTime.UtcNow.AddDays(-_daysOfDeleteUnconfirmedUsers);

        var unconfirmedUsers = await _context
            .Users.Where(u => !u.EmailConfirmed && u.RegisteredAt < cutoffDate)
            .Include(u => u.VerificationCodes)
            .ToListAsync();

        if (!unconfirmedUsers.Any())
        {
            Log.Information("No se encontraron usuarios no confirmados para eliminar");
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

        Log.Information($"Eliminados {unconfirmedUsers.Count} usuarios no confirmados");
        return unconfirmedUsers.Count;
    }

    /// <summary>
    /// Verifica si un usuario existe por su correo electrónico.
    /// </summary>
    /// <param name="email">Correo electrónico a buscar.</param>
    /// <returns>True si existe, false si no.</returns>
    public async Task<bool> ExistsByEmailAsync(string email)
    {
        return await _context.Users.AnyAsync(u => u.Email == email);
    }

    /// <summary>
    /// Verifica si un usuario existe por su RUT.
    /// </summary>
    /// <param name="rut">RUT a buscar.</param>
    /// <returns>True si existe, false si no.</returns>
    public async Task<bool> ExistsByRutAsync(string rut)
    {
        return await _context.Users.AnyAsync(u => u.Rut == rut);
    }

    /// <summary>
    /// Obtiene un usuario por su correo electrónico.
    /// </summary>
    /// <param name="email">Correo electrónico del usuario.</param>
    /// <returns>Usuario encontrado o null si no existe.</returns>
    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _userManager.FindByEmailAsync(email);
    }

    /// <summary>
    /// Obtiene un usuario por su ID.
    /// </summary>
    /// <param name="id">ID del usuario.</param>
    /// <returns>Usuario encontrado o null si no existe.</returns>
    public async Task<User?> GetByIdAsync(int id)
    {
        return await _userManager.FindByIdAsync(id.ToString());
    }

    /// <summary>
    /// Obtiene un usuario por su RUT.
    /// </summary>
    /// <param name="rut">RUT del usuario.</param>
    /// <param name="trackChanges">Indica si se debe rastrear cambios.</param>
    /// <returns>Usuario encontrado o null si no existe.</returns>
    public async Task<User?> GetByRutAsync(string rut, bool trackChanges = false)
    {
        if (trackChanges)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Rut == rut);
        }

        return await _context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Rut == rut);
    }

    /// <summary>
    /// Obtiene el rol del usuario.
    /// </summary>
    /// <param name="user">Usuario a consultar.</param>
    /// <returns>Nombre del rol del usuario.</returns>
    public async Task<string> GetUserRoleAsync(User user)
    {
        var roles = await _userManager.GetRolesAsync(user);
        return roles.FirstOrDefault()!;
    }

    public async Task<List<User>> GetUnconfirmedUsersAsync()
    {
        return await _context.Users.Where(u => !u.EmailConfirmed).ToListAsync();
    }

    /// <summary>
    /// Obtiene todos los usuarios registrados.
    /// </summary>
    /// <returns>Lista de usuarios existentes.</returns>
    public async Task<List<User>> GetAllAsync()
    {
        return await _context.Users.ToListAsync();
    }
}
