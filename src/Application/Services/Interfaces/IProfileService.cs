using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TiendaUcnApi.src.Application.DTO;

namespace TiendaUcnApi.src.Application.Services.Interfaces;
public interface IProfileService
{
    Task<ProfileDTO> GetProfileAsync(int userId);
    Task<bool> UpdateProfileAsync(int userId, UpdateProfileDTO dto);
    Task<bool> ChangePasswordAsync(int userId, ChangePasswordDTO dto);
}