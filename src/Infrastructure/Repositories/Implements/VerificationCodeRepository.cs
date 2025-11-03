using Microsoft.EntityFrameworkCore;
using TiendaUcnApi.src.Domain.Models;
using TiendaUcnApi.src.Infrastructure.Data;
using TiendaUcnApi.src.Infrastructure.Repositories.Interfaces;

namespace TiendaUcnApi.src.Infrastructure.Repositories.Implements;

/// <summary>
/// Implementation of the verification code repository.
/// Handles database operations for email verification codes and password reset codes.
/// </summary>
public class VerificationCodeRepository : IVerificationCodeRepository
{
    /// <summary>
    /// Application database context.
    /// </summary>
    private readonly AppDbContext _context;

    /// <summary>
    /// Initializes a new instance of the <see cref="VerificationCodeRepository"/> class.
    /// </summary>
    /// <param name="dataContext">Database context.</param>
    public VerificationCodeRepository(AppDbContext dataContext)
    {
        _context = dataContext;
    }

    /// <summary>
    /// Creates a new verification code.
    /// </summary>
    /// <param name="verificationCode">The verification code to create.</param>
    /// <returns>The created verification code.</returns>
    public async Task<VerificationCode> CreateAsync(VerificationCode verificationCode)
    {
        await _context.VerificationCodes.AddAsync(verificationCode);
        await _context.SaveChangesAsync();
        return verificationCode;
    }

    /// <summary>
    /// Deletes a verification code by user ID and code type.
    /// </summary>
    /// <param name="id">The user ID.</param>
    /// <param name="codeType">The verification code type.</param>
    /// <returns>True if deleted successfully, false if it didn't exist.</returns>
    public async Task<bool> DeleteByUserIdAsync(int id, CodeType codeType)
    {
        await _context
            .VerificationCodes.Where(vc => vc.UserId == id && vc.CodeType == codeType)
            .ExecuteDeleteAsync();
        var exists = await _context.VerificationCodes.AnyAsync(vc =>
            vc.UserId == id && vc.CodeType == codeType
        );
        return !exists;
    }

    /// <summary>
    /// Deletes all verification codes associated with a user.
    /// </summary>
    /// <param name="userId">The user ID.</param>
    /// <returns>The number of deleted verification codes.</returns>
    public async Task<int> DeleteByUserIdAsync(int userId)
    {
        var codes = await _context.VerificationCodes.Where(vc => vc.UserId == userId).ToListAsync();
        _context.VerificationCodes.RemoveRange(codes);
        await _context.SaveChangesAsync();
        return codes.Count;
    }

    /// <summary>
    /// Retrieves the latest verification code by user ID and code type.
    /// </summary>
    /// <param name="userId">The user ID.</param>
    /// <param name="codeType">The verification code type.</param>
    /// <returns>The latest verification code found, or null if it doesn't exist.</returns>
    public async Task<VerificationCode?> GetLatestByUserIdAsync(int userId, CodeType codeType)
    {
        return await _context
            .VerificationCodes.Where(vc => vc.UserId == userId && vc.CodeType == codeType)
            .OrderByDescending(vc => vc.CreatedAt)
            .FirstOrDefaultAsync();
    }

    /// <summary>
    /// Increases the attempt counter of a verification code.
    /// </summary>
    /// <param name="userId">The user ID.</param>
    /// <param name="codeType">The verification code type.</param>
    /// <returns>The incremented attempt count.</returns>
    public async Task<int> IncreaseAttemptsAsync(int userId, CodeType codeType)
    {
        await _context
            .VerificationCodes.Where(vc => vc.UserId == userId && vc.CodeType == codeType)
            .ExecuteUpdateAsync(setters =>
                setters.SetProperty(vc => vc.AttemptCount, vc => vc.AttemptCount + 1)
            );
        var newCount = await _context
            .VerificationCodes.AsNoTracking()
            .Where(vc => vc.UserId == userId && vc.CodeType == codeType)
            .Select(vc => vc.AttemptCount)
            .FirstOrDefaultAsync();
        return newCount;
    }

    /// <summary>
    /// Updates an existing verification code.
    /// </summary>
    /// <param name="verificationCode">The verification code to update.</param>
    /// <returns>The updated verification code.</returns>
    public async Task<VerificationCode?> UpdateAsync(VerificationCode verificationCode)
    {
        await _context
            .VerificationCodes.Where(vc => vc.Id == verificationCode.Id)
            .ExecuteUpdateAsync(setters =>
                setters
                    .SetProperty(vc => vc.Code, verificationCode.Code)
                    .SetProperty(vc => vc.AttemptCount, verificationCode.AttemptCount)
                    .SetProperty(vc => vc.ExpiryDate, verificationCode.ExpiryDate)
            );
        return await _context
            .VerificationCodes.AsNoTracking()
            .FirstOrDefaultAsync(vc => vc.Id == verificationCode.Id);
    }
}
