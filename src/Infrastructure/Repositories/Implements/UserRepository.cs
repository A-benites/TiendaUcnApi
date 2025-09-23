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
    private readonly AppDbContext _context;
    private readonly UserManager<User> _userManager;
    private readonly int _daysOfDeleteUnconfirmedUsers;
    private readonly IVerificationCodeRepository _verificationCodeRepository;

    // Se mantiene el constructor que inyecta todas las dependencias necesarias.
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
    public async Task<bool> CheckPasswordAsync(User user, string password)
    {
        return await _userManager.CheckPasswordAsync(user, password);
    }

    /// <summary>
    /// Confirma el correo electrónico del usuario.
    /// </summary>
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
    public async Task<bool> CreateAsync(User user, string password)
    {
        // Se mantiene la versión con transacciones para asegurar la integridad de los datos.
        await using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            // 1. Intenta crear el usuario
            var userResult = await _userManager.CreateAsync(user, password);
            if (!userResult.Succeeded)
            {
                await transaction.RollbackAsync();
                return false;
            }

            // 2. Intenta asignar el rol "Cliente"
            var roleResult = await _userManager.AddToRoleAsync(user, "Cliente");
            if (!roleResult.Succeeded)
            {
                await transaction.RollbackAsync();
                return false;
            }

            // 3. Si todo es exitoso, confirma la transacción
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
    public async Task<bool> DeleteAsync(int userId)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        var result = await _userManager.DeleteAsync(user!);
        return result.Succeeded;
    }

    /// <summary>
    /// Elimina usuarios no confirmados y sus códigos de verificación asociados.
    /// </summary>
    public async Task<int> DeleteUnconfirmedAsync()
    {
        Log.Information("Iniciando eliminación de usuarios no confirmados");

        var cutoffDate = DateTime.UtcNow.AddDays(_daysOfDeleteUnconfirmedUsers);

        var unconfirmedUsers = await _context
            .Users.Where(u => !u.EmailConfirmed && u.RegisteredAt < cutoffDate)
            .Include(u => u.VerificationCodes)
            .ToListAsync();

        if (!unconfirmedUsers.Any())
        {
            Log.Information("No se encontraron usuarios no confirmados para eliminar");
            return 0;
        }
        
        // Se mantiene la lógica que elimina los códigos de verificación antes de borrar al usuario.
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
    public async Task<bool> ExistsByEmailAsync(string email)
    {
        return await _context.Users.AnyAsync(u => u.Email == email);
    }

    /// <summary>
    /// Verifica si un usuario existe por su RUT.
    /// </summary>
    public async Task<bool> ExistsByRutAsync(string rut)
    {
        return await _context.Users.AnyAsync(u => u.Rut == rut);
    }

    /// <summary>
    /// Obtiene un usuario por su correo electrónico.
    /// </summary>
    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _userManager.FindByEmailAsync(email);
    }

    /// <summary>
    /// Obtiene un usuario por su ID.
    /// </summary>
    public async Task<User?> GetByIdAsync(int id)
    {
        return await _userManager.FindByIdAsync(id.ToString());
    }

    /// <summary>
    /// Obtiene un usuario por su RUT.
    /// </summary>
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
    public async Task<string> GetUserRoleAsync(User user)
    {
        var roles = await _userManager.GetRolesAsync(user);
        return roles.FirstOrDefault()!;
    }
}