using TiendaUcnApi.src.Domain.Models;

namespace TiendaUcnApi.src.Infrastructure.Repositories.Interfaces;

// Define el contrato para el repositorio de usuarios.
// Cualquier clase que implemente esta interfaz DEBE proporcionar estos métodos.
public interface IUserRepository
{
    /// <summary>
    /// Agrega un nuevo usuario a la base de datos.
    /// </summary>
    /// <param name="user">La entidad de usuario a agregar.</param>
    Task AddAsync(User user);

    /// <summary>
    /// Busca un usuario por su dirección de correo electrónico.
    /// </summary>
    /// <param name="email">El email del usuario a buscar.</param>
    /// <returns>La entidad User si se encuentra, de lo contrario null.</returns>
    Task<User?> GetByEmailAsync(string email);

    /// <summary>
    /// Busca un usuario por su RUT.
    /// </summary>
    /// <param name="rut">El RUT del usuario a buscar.</param>
    /// <returns>La entidad User si se encuentra, de lo contrario null.</returns>
    Task<User?> GetByRutAsync(string rut);

}