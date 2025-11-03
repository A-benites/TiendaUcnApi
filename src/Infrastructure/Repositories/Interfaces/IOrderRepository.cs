using TiendaUcnApi.src.Application.DTO.OrderDTO;
using TiendaUcnApi.src.Domain.Models;

namespace TiendaUcnApi.src.Infrastructure.Repositories.Interfaces;

/// <summary>
/// Repository interface for order data access operations.
/// Handles order creation, retrieval, and status management.
/// </summary>
public interface IOrderRepository
{
    /// <summary>
    /// Creates a new order in the database from a shopping cart.
    /// </summary>
    /// <param name="order">The order entity to create.</param>
    /// <returns>The created order with generated ID and code.</returns>
    Task<Order> CreateAsync(Order order);

    /// <summary>
    /// Retrieves all orders for a specific user.
    /// </summary>
    /// <param name="userId">The user identifier.</param>
    /// <returns>Collection of orders belonging to the user.</returns>
    Task<IEnumerable<Order>> GetAllByUser(int userId);

    /// <summary>
    /// Retrieves paginated orders for a specific user.
    /// </summary>
    /// <param name="userId">The user identifier.</param>
    /// <param name="page">Page number (1-indexed).</param>
    /// <param name="pageSize">Number of orders per page.</param>
    /// <returns>Tuple containing orders collection and total count.</returns>
    Task<(IEnumerable<Order> Orders, int TotalCount)> GetAllByUserPaginated(
        int userId,
        int page,
        int pageSize
    );

    /// <summary>
    /// Retrieves a specific order by its identifier.
    /// </summary>
    /// <param name="id">The order identifier.</param>
    /// <returns>Order entity or null if not found.</returns>
    Task<Order?> GetByIdAsync(int id);

    /// <summary>
    /// Retrieves all orders with filtering and pagination (admin only).
    /// </summary>
    /// <param name="filter">Filter criteria including search, status, date range, and pagination.</param>
    /// <returns>Tuple containing filtered orders collection and total count.</returns>
    Task<(IEnumerable<Order> Orders, int TotalCount)> GetAllAsync(OrderFilterDTO filter);

    /// <summary>
    /// Updates the status of an order.
    /// </summary>
    /// <param name="id">The order identifier to update.</param>
    /// <param name="status">The new order status.</param>
    /// <param name="adminId">Administrator identifier performing the update.</param>
    /// <returns>The updated order entity.</returns>
    Task<Order> UpdateStatusAsync(int id, OrderStatus status, int adminId);
}
