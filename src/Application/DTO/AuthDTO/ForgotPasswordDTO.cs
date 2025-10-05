using System.ComponentModel.DataAnnotations;

namespace TiendaUcnApi.src.Application.DTO.AuthDTO;

public class ForgotPasswordDTO
{
    [EmailAddress]
    public required string Email { get; set; }
}