using Mapster;
using TiendaUcnApi.src.Application.DTO.AuthDTO;
using TiendaUcnApi.src.Domain.Models;

namespace TiendaUcnApi.src.Application.Mappers;

/// <summary>
/// Configures mappings between User entities and User DTOs using Mapster.
/// Handles transformations for user registration and authentication operations.
/// </summary>
public class UserMapper
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UserMapper"/> class.
    /// </summary>
    public UserMapper() { }

    /// <summary>
    /// Configures all user-related mappings.
    /// </summary>
    public void ConfigureAllMappings()
    {
        ConfigureAuthMappings();
    }

    /// <summary>
    /// Configures the mapping from RegisterDTO to User entity.
    /// Maps registration data to create a new user account with email unconfirmed by default.
    /// </summary>
    public void ConfigureAuthMappings()
    {
        TypeAdapterConfig<RegisterDTO, User>
            .NewConfig()
            .Map(dest => dest.UserName, src => src.Email)
            .Map(dest => dest.Email, src => src.Email)
            .Map(dest => dest.FirstName, src => src.FirstName)
            .Map(dest => dest.LastName, src => src.LastName)
            .Map(dest => dest.Rut, src => src.Rut)
            .Map(dest => dest.BirthDate, src => src.BirthDate)
            .Map(dest => dest.PhoneNumber, src => src.PhoneNumber)
            .Map(dest => dest.Gender, src => src.Gender)
            .Map(dest => dest.EmailConfirmed, src => false);
    }
}
