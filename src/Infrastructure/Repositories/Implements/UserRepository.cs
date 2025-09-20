using Microsoft.EntityFrameworkCore;
using TiendaUcnApi.src.Domain.Models;
using TiendaUcnApi.src.Infrastructure.Data;
using TiendaUcnApi.src.Infrastructure.Repositories.Interfaces;

namespace TiendaUcnApi.src.Infrastructure.Repositories.Implements;

// Implementación concreta del repositorio de usuarios.
// Utiliza Entity Framework Core para interactuar con la base de datos.
public class UserRepository : IUserRepository
{
    private readonly AppDbContext _context;

    // El DataContext es inyectado por el contenedor de dependencias de ASP.NET Core.
    public UserRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(User user)
    {
        await _context.Users.AddAsync(user);
        // NOTA: El Guardado de cambios (SaveChanges) se manejará en una capa superior
        // o en una unidad de trabajo para agrupar operaciones.
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        // MEJORA: Usar String.Equals con OrdinalIgnoreCase es más eficiente
        // para comparaciones de texto sin distinción de mayúsculas y minúsculas
        // porque puede aprovechar mejor los índices de la base de datos.
        return await _context.Users.FirstOrDefaultAsync(u =>
            string.Equals(u.Email, email, StringComparison.OrdinalIgnoreCase));
    }

    public async Task<User?> GetByRutAsync(string rut)
    {
        // Busca el primer usuario que coincida con el RUT.
        return await _context.Users.FirstOrDefaultAsync(u => u.Rut == rut);
    }
}