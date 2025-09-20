using TiendaUcnApi.src.Domain.Models;

namespace TiendaUcnApi.src.Infrastructure.Repositories.Interfaces;

// Interfaz simplificada para la rama de registro.
public interface IVerificationCodeRepository
{
    /// <summary>
    /// Prepara un nuevo código de verificación para ser agregado a la base de datos.
    /// </summary>
    /// <param name="verificationCode">La entidad del código a agregar.</param>
    Task AddAsync(VerificationCode verificationCode);

    /// <summary>
    /// Guarda todos los cambios pendientes en la base de datos.
    /// Implementa el patrón Unit of Work.
    /// </summary>
    /// <returns>True si los cambios se guardaron, de lo contrario false.</returns>
    Task<bool> SaveChangesAsync();
}