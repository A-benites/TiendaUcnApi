using System.ComponentModel.DataAnnotations;
using TiendaUcnApi.src.Domain.Models;

/// <summary>
/// DTO para mostrar los datos del perfil de usuario.
/// </summary>
public class ProfileDTO
{
    /// <summary>
    /// Correo electrónico del usuario.
    /// </summary>
    [Required(ErrorMessage = "El email es requerido.")]
    [EmailAddress(ErrorMessage = "El Correo electrónico no es válido.")]
    public required string Email { get; set; }

    /// <summary>
    /// Primer nombre del usuario.
    /// </summary>
    [Required(ErrorMessage = "El primer nombre es requerido")]
    public required string FirstName { get; set; }

    /// <summary>
    /// Apellido del usuario.
    /// </summary>
    [Required(ErrorMessage = "El apellido es requerido")]
    public required string LastName { get; set; }

    /// <summary>
    /// RUT del usuario.
    /// </summary>
    [Required(ErrorMessage = "El RUT es requerido")]
    public required string Rut { get; set; }

    /// <summary>
    /// Género del usuario.
    /// </summary>
    [Required(ErrorMessage = "El género es requerido")]
    public Gender Gender { get; set; }

    /// <summary>
    /// Fecha de nacimiento del usuario.
    /// </summary>
    [Required(ErrorMessage = "La fecha de nacimiento es requerida")]
    public DateOnly BirthDate { get; set; }

    /// <summary>
    /// Número de teléfono del usuario.
    /// </summary>
    public string? PhoneNumber { get; set; }
}