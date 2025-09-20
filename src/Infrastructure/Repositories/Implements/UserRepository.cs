using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;
using TiendaUcnApi.src.Domain.Models;
using TiendaUcnApi.src.Infrastructure.Data;
using TiendaUcnApi.src.Infrastructure.Repositories.Interfaces;

namespace TiendaUcnApi.src.Infrastructure.Repositories.Implements;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _context;
    private readonly UserManager<User> _userManager;

    // Inyectamos UserManager, que ahora está disponible gracias a la configuración
    // que hicimos en Program.cs.
    public UserRepository(AppDbContext context, UserManager<User> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    // Usamos UserManager para crear el usuario. Se encarga del hasheo y de guardar.
    public async Task<bool> CreateAsync(User user, string password)
    {
        var userResult = await _userManager.CreateAsync(user, password);
        if (userResult.Succeeded)
        {
            // Asegúrate que el nombre del rol coincida con el que definiste en tu seeder/configuración.
            // El taller lo llama "Cliente".
            var roleResult = await _userManager.AddToRoleAsync(user, "Cliente");

            if (!roleResult.Succeeded)
            {
                // Si falla la asignación de rol, es bueno loguearlo y quizás eliminar al usuario.
                Log.Error(
                    $"Usuario {user.Email} creado, pero falló la asignación del rol 'Cliente'."
                );
                await _userManager.DeleteAsync(user); // Revertir creación
                return false;
            }
            return true;
        }
        else
        {
            // Loguear los errores específicos de Identity es muy útil para depurar.
            foreach (var error in userResult.Errors)
            {
                Log.Warning($"Error al crear usuario {user.Email}: {error.Description}");
            }
        }
        return false;
    }

    // UserManager tiene un método optimizado para buscar por email.
    public async Task<User?> GetByEmailAsync(string email)
    {
        // Este método ya maneja la comparación sin distinguir mayúsculas/minúsculas.
        return await _userManager.FindByEmailAsync(email);
    }

    // UserManager no tiene un método directo para buscar por RUT,
    // así que mantenemos nuestra implementación con el DbContext.
    public async Task<User?> GetByRutAsync(string rut)
    {
        return await _context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Rut == rut);
    }

    // Podemos usar el mismo método de UserManager para verificar si existe.
    public async Task<bool> ExistsByEmailAsync(string email)
    {
        var user = await _userManager.FindByEmailAsync(email);
        return user != null;
    }

    // Para el RUT, usamos el DbContext.
    public async Task<bool> ExistsByRutAsync(string rut)
    {
        return await _context.Users.AnyAsync(u => u.Rut == rut);
    }
}