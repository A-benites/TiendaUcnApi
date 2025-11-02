using TiendaUcnApi.src.Domain.Models;

namespace TiendaUcnApi.src.Application.Services;

/// <summary>
/// Validador de transiciones de estado para órdenes.
/// Implements R123 rubric requirement: state machine with valid transitions.
/// </summary>
public static class OrderStatusTransitionValidator
{
    /// <summary>
    /// Define las transiciones válidas de estado.
    /// </summary>
    private static readonly Dictionary<OrderStatus, HashSet<OrderStatus>> ValidTransitions =
        new()
        {
            // Desde Pending se puede ir a Processing o Cancelled
            [OrderStatus.Pending] = new HashSet<OrderStatus>
            {
                OrderStatus.Processing,
                OrderStatus.Cancelled,
            },
            // Desde Processing se puede ir a Shipped o Cancelled
            [OrderStatus.Processing] = new HashSet<OrderStatus>
            {
                OrderStatus.Shipped,
                OrderStatus.Cancelled,
            },
            // Desde Shipped solo se puede ir a Delivered
            [OrderStatus.Shipped] = new HashSet<OrderStatus> { OrderStatus.Delivered, },
            // Delivered y Cancelled son estados finales (no permiten transiciones)
            [OrderStatus.Delivered] = new HashSet<OrderStatus>(),
            [OrderStatus.Cancelled] = new HashSet<OrderStatus>(),
        };

    /// <summary>
    /// Valida si una transición de estado es válida.
    /// </summary>
    /// <param name="currentStatus">Estado actual de la orden.</param>
    /// <param name="newStatus">Nuevo estado deseado.</param>
    /// <returns>True si la transición es válida, false en caso contrario.</returns>
    public static bool IsValidTransition(OrderStatus currentStatus, OrderStatus newStatus)
    {
        // Si el estado no cambia, es válido (idempotencia)
        if (currentStatus == newStatus)
            return true;

        // Verificar si existe una transición válida
        return ValidTransitions.ContainsKey(currentStatus)
            && ValidTransitions[currentStatus].Contains(newStatus);
    }

    /// <summary>
    /// Obtiene un mensaje de error descriptivo para una transición inválida.
    /// </summary>
    /// <param name="currentStatus">Estado actual de la orden.</param>
    /// <param name="newStatus">Nuevo estado deseado.</param>
    /// <returns>Mensaje de error descriptivo.</returns>
    public static string GetTransitionErrorMessage(
        OrderStatus currentStatus,
        OrderStatus newStatus
    )
    {
        if (ValidTransitions[currentStatus].Count == 0)
        {
            return $"No se puede cambiar el estado de una orden {currentStatus}. Es un estado final.";
        }

        var validStates = string.Join(", ", ValidTransitions[currentStatus]);
        return $"Transición inválida: no se puede cambiar de {currentStatus} a {newStatus}. Estados válidos desde {currentStatus}: {validStates}.";
    }
}
