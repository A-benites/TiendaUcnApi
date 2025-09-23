using System.ComponentModel.DataAnnotations;
using TiendaUcnApi.src.Domain.Models;

namespace TiendaUcnApi.src.Application.DTO;

public class UpdateProfileDTO
{
    [StringLength(50, MinimumLength = 2)]
    public string? FirstName { get; set; }

    [StringLength(50, MinimumLength = 2)]
    public string? LastName { get; set; }

    public Gender? Gender { get; set; }

    public DateOnly? BirthDate { get; set; }
}