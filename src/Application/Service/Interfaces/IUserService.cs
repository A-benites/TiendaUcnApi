using TiendaUcnApi.src.Application.DTO.AuthDTO;

namespace TiendaUcnApi.src.Application.Services.Interfaces;

// Define el contrato para el servicio que gestiona la lógica de negocio de los usuarios.
public interface IUserService
{
    /// <summary>
    /// Registra un nuevo usuario en el sistema.
    /// </summary>
    /// <param name="registerDto">Los datos del usuario a registrar.</param>
    Task RegisterAsync(RegisterDTO registerDto);
}