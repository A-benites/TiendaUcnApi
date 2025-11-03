using TiendaUcnApi.src.Application.DTO;
using TiendaUcnApi.src.Application.DTO.OrderDTO;

namespace TiendaUcnApi.src.Application.Services.Interfaces;

/// <summary>
/// Service interface for order management operations.
/// </summary>
public interface IOrderService
{
    /// <summary>
    /// Creates an order from the user's shopping cart.
    /// </summary>
    /// <param name="buyerId">Buyer identifier (for anonymous users).</param>
    /// <param name="userId">User identifier (for authenticated users).</param>
    /// <returns>Response containing the created order.</returns>
    Task<GenericResponse<OrderDTO>> CreateAsync(string buyerId, int userId);

    /// <summary>
    /// Retrieves all orders for a specific user.
    /// </summary>
    /// <param name="userId">User identifier.</param>
    /// <returns>List of orders belonging to the user.</returns>
    Task<GenericResponse<List<OrderDTO>>> GetAllByUser(int userId);

    /// <summary>
    /// Retrieves all orders for a specific user with pagination.
    /// </summary>
    /// <param name="userId">User identifier.</param>
    /// <param name="filter">Pagination and filtering parameters.</param>
    /// <returns>Paginated list of orders belonging to the user.</returns>
    Task<GenericResponse<OrderListDTO>> GetAllByUserPaginated(
        int userId,
        UserOrderFilterDTO filter
    );

    /// <summary>
    /// Retrieves order details by ID (only if it belongs to the user).
    /// </summary>
    /// <param name="orderId">Order identifier.</param>
    /// <param name="userId">User identifier for ownership validation.</param>
    /// <returns>Order details if the user owns the order.</returns>
    Task<GenericResponse<OrderDTO>> GetOrderDetailByIdAsync(int orderId, int userId);

    /// <summary>
    /// Retrieves all orders with filtering options (admin only).
    /// </summary>
    /// <param name="filter">Search and filter parameters.</param>
    /// <returns>Paginated list of all orders.</returns>
    Task<GenericResponse<OrderListDTO>> GetAllOrdersAsync(OrderFilterDTO filter);

    /// <summary>
    /// Retrieves order details by ID (admin only, no ownership check).
    /// </summary>
    /// <param name="orderId">Order identifier.</param>
    /// <returns>Order details.</returns>
    Task<GenericResponse<OrderDTO>> GetOrderByIdAsync(int orderId);

    /// <summary>
    /// Updates an order's status with validation (admin only).
    /// </summary>
    /// <param name="orderId">Order identifier to update.</param>
    /// <param name="dto">DTO containing the new status.</param>
    /// <param name="adminId">Administrator identifier performing the change.</param>
    /// <returns>Updated order details.</returns>
    Task<GenericResponse<OrderDTO>> UpdateOrderStatusAsync(
        int orderId,
        UpdateOrderStatusDTO dto,
        int adminId
    );
}
