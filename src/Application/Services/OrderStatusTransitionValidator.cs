using TiendaUcnApi.src.Domain.Models;

namespace TiendaUcnApi.src.Application.Services;

/// <summary>
/// Validator for order status transitions.
/// Implements R123 rubric requirement: state machine with valid transitions.
/// Ensures orders can only move through valid status sequences.
/// </summary>
public static class OrderStatusTransitionValidator
{
    /// <summary>
    /// Defines valid status transitions.
    /// Maps each current status to the set of allowed next statuses.
    /// </summary>
    private static readonly Dictionary<OrderStatus, HashSet<OrderStatus>> ValidTransitions = new()
    {
        // From Pending can go to Processing or Cancelled
        [OrderStatus.Pending] = new HashSet<OrderStatus>
        {
            OrderStatus.Processing,
            OrderStatus.Cancelled,
        },
        // From Processing can go to Shipped or Cancelled
        [OrderStatus.Processing] = new HashSet<OrderStatus>
        {
            OrderStatus.Shipped,
            OrderStatus.Cancelled,
        },
        // From Shipped can only go to Delivered
        [OrderStatus.Shipped] = new HashSet<OrderStatus> { OrderStatus.Delivered },
        // Delivered and Cancelled are final states (no transitions allowed)
        [OrderStatus.Delivered] = new HashSet<OrderStatus>(),
        [OrderStatus.Cancelled] = new HashSet<OrderStatus>(),
    };

    /// <summary>
    /// Validates if a status transition is valid.
    /// </summary>
    /// <param name="currentStatus">Current order status.</param>
    /// <param name="newStatus">Desired new status.</param>
    /// <returns>True if the transition is valid, false otherwise.</returns>
    public static bool IsValidTransition(OrderStatus currentStatus, OrderStatus newStatus)
    {
        // If the status doesn't change, it's valid (idempotency)
        if (currentStatus == newStatus)
            return true;

        // Verify if a valid transition exists
        return ValidTransitions.ContainsKey(currentStatus)
            && ValidTransitions[currentStatus].Contains(newStatus);
    }

    /// <summary>
    /// Gets a descriptive error message for an invalid transition.
    /// </summary>
    /// <param name="currentStatus">Current order status.</param>
    /// <param name="newStatus">Desired new status.</param>
    /// <returns>Descriptive error message.</returns>
    public static string GetTransitionErrorMessage(OrderStatus currentStatus, OrderStatus newStatus)
    {
        if (ValidTransitions[currentStatus].Count == 0)
        {
            return $"Cannot change the status of a {currentStatus} order. It is a final state.";
        }

        var validStates = string.Join(", ", ValidTransitions[currentStatus]);
        return $"Invalid transition: cannot change from {currentStatus} to {newStatus}. Valid states from {currentStatus}: {validStates}.";
    }
}
