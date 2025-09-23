using System.ComponentModel.DataAnnotations;
using TiendaUcnApi.src.Domain.Models;

namespace TiendaUcnApi.src.Application.DTO;
public class ProfileDTO
{
    [Required(ErrorMessage = "El email es requerido.")]
    [EmailAddress(ErrorMessage = "El Correo electrónico no es válido.")]
    public required string Email { get; set; }

    [Required(ErrorMessage = "El primer nombre es requerido")]
    public required string FirstName { get; set; }

    [Required(ErrorMessage = "El apellido es requerido")]
    public required string LastName { get; set; }

    [Required(ErrorMessage = "El RUT es requerido")]
    public required string Rut { get; set; }

    [Required(ErrorMessage = "El género es requerido")]
    public Gender Gender { get; set; }

    [Required(ErrorMessage = "La fecha de nacimiento es requerida")]
    public DateOnly BirthDate { get; set; }
}