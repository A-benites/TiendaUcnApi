using System.ComponentModel.DataAnnotations;
using TiendaUcnApi.src.Domain.Models;

/// <summary>
/// Data Transfer Object for displaying user profile data.
/// </summary>
public class ProfileDTO
{
    /// <summary>
    /// User's email address.
    /// </summary>
    [Required(ErrorMessage = "El email es requerido.")]
    [EmailAddress(ErrorMessage = "El Correo electrónico no es válido.")]
    public required string Email { get; set; }

    /// <summary>
    /// User's first name.
    /// </summary>
    [Required(ErrorMessage = "El primer nombre es requerido")]
    public required string FirstName { get; set; }

    /// <summary>
    /// User's last name.
    /// </summary>
    [Required(ErrorMessage = "El apellido es requerido")]
    public required string LastName { get; set; }

    /// <summary>
    /// Chilean national identification number (RUT).
    /// </summary>
    [Required(ErrorMessage = "El RUT es requerido")]
    public required string Rut { get; set; }

    /// <summary>
    /// User's gender.
    /// </summary>
    [Required(ErrorMessage = "El género es requerido")]
    public Gender Gender { get; set; }

    /// <summary>
    /// User's birth date.
    /// </summary>
    [Required(ErrorMessage = "La fecha de nacimiento es requerida")]
    public DateOnly BirthDate { get; set; }

    /// <summary>
    /// User's phone number (optional).
    /// </summary>
    public string? PhoneNumber { get; set; }
}
