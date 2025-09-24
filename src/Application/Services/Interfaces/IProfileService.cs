using TiendaUcnApi.src.Application.DTO;
using TiendaUcnApi.src.Application.DTO.AuthDTO;

namespace TiendaUcnApi.src.Application.Services.Interfaces;

public interface IProfileService
{
    Task<ProfileDTO> GetProfileAsync(int userId);

    Task<string> UpdateProfileAsync(int userId, UpdateProfileDTO dto);

    Task ChangePasswordAsync(int userId, ChangePasswordDTO dto);

    Task<string> VerifyEmailChangeAsync(int userId, VerifyEmailChangeDTO dto);
}