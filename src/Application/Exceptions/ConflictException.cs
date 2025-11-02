namespace TiendaUcnApi.src.Application.Exceptions;

/// <summary>
/// Excepción que representa un conflicto de recursos (HTTP 409).
/// Se lanza cuando una operación no puede completarse debido a un conflicto con el estado actual del recurso.
/// </summary>
public class ConflictException : Exception
{
    public ConflictException(string message)
        : base(message) { }

    public ConflictException(string message, Exception innerException)
        : base(message, innerException) { }
}
