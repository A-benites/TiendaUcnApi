namespace TiendaUcnApi.src.Application.Exceptions;

/// <summary>
/// Exception representing a resource conflict (HTTP 409).
/// Thrown when an operation cannot be completed due to a conflict with the current state of the resource.
/// Typically used for duplicate resources, version conflicts, or business rule violations.
/// </summary>
public class ConflictException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ConflictException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public ConflictException(string message)
        : base(message) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="ConflictException"/> class with a specified error message
    /// and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public ConflictException(string message, Exception innerException)
        : base(message, innerException) { }
}
