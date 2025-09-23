using Microsoft.AspNetCore.Identity;
using TiendaUcnApi.src.Application.DTO;
using TiendaUcnApi.src.Application.Services.Interfaces;
using TiendaUcnApi.src.Domain.Models;
using TiendaUcnApi.src.Infrastructure.Data;

namespace TiendaUcnApi.src.Application.Services.Implements;
public class ProfileService : IProfileService
{
    private readonly AppDbContext _context;
    private readonly IPasswordHasher<User> _passwordHasher;

    public ProfileService(AppDbContext context, IPasswordHasher<User> passwordHasher)
    {
        _context = context;
        _passwordHasher = passwordHasher;
    }

    public async Task<ProfileDTO> GetProfileAsync(Guid userId)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null) throw new Exception("User not found");

        return new ProfileDTO
        {
            Email = user.Email,
            FirstName = user.FirstName
        };
    }

    public async Task<bool> UpdateProfileAsync(Guid userId, UpdateProfileDTO dto)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null) return false;

        user.FirstName = dto.FirstName;
        _context.Users.Update(user);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ChangePasswordAsync(Guid userId, ChangePasswordDTO dto)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null) return false;

        var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, dto.OldPassword);
        if (result == PasswordVerificationResult.Failed) return false;

        user.PasswordHash = _passwordHasher.HashPassword(user, dto.NewPassword);
        _context.Users.Update(user);
        await _context.SaveChangesAsync();
        return true;
    }
}
