using TiendaUcnApi.src.Domain.Models;
using TiendaUcnApi.src.Infrastructure.Data;
using TiendaUcnApi.src.Infrastructure.Repositories.Interfaces;

namespace TiendaUcnApi.src.Infrastructure.Repositories.Implements;

// Implementación concreta para el repositorio de códigos de verificación.
public class VerificationCodeRepository : IVerificationCodeRepository
{
    private readonly AppDbContext _context;

    public VerificationCodeRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(VerificationCode verificationCode)
    {
        await _context.VerificationCodes.AddAsync(verificationCode);
    }
}