namespace TiendaUcnApi.src.Application.DTO;

/// <summary>
/// Class representing a generic application response.
/// </summary>
/// <typeparam name="T">Type of data contained in the response.</typeparam>
/// <param name="message">Response message.</param>
/// <param name="data">Response data (optional).</param>
public class GenericResponse<T>(string message, T? data = default)
{
    /// <summary>
    /// Main response message.
    /// </summary>
    public string Message { get; set; } = message;

    /// <summary>
    /// Response data (may be null).
    /// </summary>
    public T? Data { get; set; } = data;
}
