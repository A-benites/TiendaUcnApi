namespace TiendaUcnApi.src.Application.DTO.BaseResponse
{
    /// <summary>
    /// Class representing error details.
    /// </summary>
    /// <param name="message">Error message.</param>
    /// <param name="details">Additional error details (optional).</param>
    public class ErrorDetail(string message, string? details = null)
    {
        /// <summary>
        /// Main error message.
        /// </summary>
        public string Message { get; set; } = message;

        /// <summary>
        /// Additional error details (optional).
        /// </summary>
        public string? Details { get; set; } = details;
    }
}
