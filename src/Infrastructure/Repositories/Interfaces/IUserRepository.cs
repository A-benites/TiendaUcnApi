using TiendaUcnApi.src.Domain.Models;

namespace TiendaUcnApi.src.Infrastructure.Repositories.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByEmailAsync(string email);
    Task<bool> ExistsByEmailAsync(string email);
    Task<bool> ExistsByRutAsync(string rut);
    Task<User?> GetByRutAsync(string rut); // Mantendremos nuestra implementación para este
    Task<bool> CreateAsync(User user, string password);
}