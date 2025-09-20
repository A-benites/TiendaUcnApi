using TiendaUcnApi.src.Domain.Models;

namespace TiendaUcnApi.src.Infrastructure.Repositories.Interfaces;
// Define el contrato para el repositorio de códigos de verificación.
public interface IVerificationCodeRepository
{
    /// <summary>
    /// Agrega un nuevo código de verificación a la base de datos.
    /// </summary>
    /// <param name="verificationCode">La entidad del código a agregar.</param>
    Task AddAsync(VerificationCode verificationCode);

    // NOTA: A futuro, se podrían agregar métodos para buscar o invalidar códigos.
    // Task<VerificationCode> GetByCodeAndUserAsync(string code, string userId);
}