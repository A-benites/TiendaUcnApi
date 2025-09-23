using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TiendaUcnApi.src.Application.DTO;

namespace TiendaUcnApi.src.Application.Services.Interfaces;
public interface IProfileService
{
    Task<ProfileDTO> GetProfileAsync(Guid userId);
    Task<bool> UpdateProfileAsync(Guid userId, UpdateProfileDTO dto);
    Task<bool> ChangePasswordAsync(Guid userId, ChangePasswordDTO dto);
}