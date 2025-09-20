using Microsoft.EntityFrameworkCore;
using TiendaUcnApi.src.Domain.Models;
using TiendaUcnApi.src.Infrastructure.Data;
using TiendaUcnApi.src.Infrastructure.Repositories.Interfaces;

namespace TiendaUcnApi.src.Infrastructure.Repositories.Implements;

public class VerificationCodeRepository : IVerificationCodeRepository
{
    private readonly AppDbContext _context;

    public VerificationCodeRepository(AppDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Agrega un nuevo código de verificación al contexto de EF Core.
    /// NOTA: No guarda los cambios en la BD, solo los prepara.
    /// </summary>
    public async Task AddAsync(VerificationCode verificationCode)
    {
        await _context.VerificationCodes.AddAsync(verificationCode);
    }

    /// <summary>
    /// El servicio llamará a este método para confirmar y guardar todas las operaciones
    /// pendientes (crear usuario, crear código) en una única transacción.
    /// </summary>
    public async Task<bool> SaveChangesAsync()
    {
        // SaveChangesAsync devuelve el número de filas afectadas.
        // Si es mayor que 0, significa que la operación fue exitosa.
        return await _context.SaveChangesAsync() > 0;
    }
}