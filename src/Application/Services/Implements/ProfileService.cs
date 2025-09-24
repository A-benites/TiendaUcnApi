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

    public async Task<ProfileDTO> GetProfileAsync(int userId)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null)
            throw new Exception("User not found");

        return new ProfileDTO
        {
            Email = user.Email!,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Rut = user.Rut,
            Gender = user.Gender,
            BirthDate = user.BirthDate,
        };
    }

    public async Task<bool> UpdateProfileAsync(int userId, UpdateProfileDTO dto)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null)
            return false;

        if (!string.IsNullOrWhiteSpace(dto.FirstName))
        {
            user.FirstName = dto.FirstName;
        }

        if (!string.IsNullOrWhiteSpace(dto.LastName))
        {
            user.LastName = dto.LastName;
        }

        if (dto.Gender.HasValue)
        {
            user.Gender = dto.Gender.Value;
        }

        if (dto.BirthDate.HasValue)
        {
            user.BirthDate = dto.BirthDate.Value;
        }

        user.UpdatedAt = DateTime.UtcNow;

        _context.Users.Update(user);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ChangePasswordAsync(int userId, ChangePasswordDTO dto)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null)
            return false;

        var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, dto.OldPassword);
        if (result == PasswordVerificationResult.Failed)
            return false;

        user.PasswordHash = _passwordHasher.HashPassword(user, dto.NewPassword);
        _context.Users.Update(user);
        await _context.SaveChangesAsync();
        return true;
    }
}