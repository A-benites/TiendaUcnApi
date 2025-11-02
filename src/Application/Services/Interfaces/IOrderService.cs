using TiendaUcnApi.src.Application.DTO;
using TiendaUcnApi.src.Application.DTO.OrderDTO;

namespace TiendaUcnApi.src.Application.Services.Interfaces;

public interface IOrderService
{
    /// <summary>
    /// Crea una orden a partir del carrito del usuario.
    /// </summary>
    /// <param name="buyerId">Identificador del comprador.</param>
    /// <param name="userId">Identificador del usuario.</param>
    /// <returns>Respuesta con la orden creada.</returns>
    Task<GenericResponse<OrderDTO>> CreateAsync(string buyerId, int userId);

    /// <summary>
    /// Obtiene todas las órdenes de un usuario.
    /// </summary>
    /// <param name="userId">Identificador del usuario.</param>
    /// <returns>Lista de órdenes del usuario.</returns>
    Task<GenericResponse<List<OrderDTO>>> GetAllByUser(int userId);

    /// <summary>
    /// Obtiene todas las órdenes de un usuario con paginación.
    /// </summary>
    /// <param name="userId">Identificador del usuario.</param>
    /// <param name="filter">Filtros de paginación.</param>
    /// <returns>Lista paginada de órdenes del usuario.</returns>
    Task<GenericResponse<OrderListDTO>> GetAllByUserPaginated(
        int userId,
        UserOrderFilterDTO filter
    );

    /// <summary>
    /// Obtiene el detalle de una orden por su ID (solo si pertenece al usuario).
    /// </summary>
    /// <param name="orderId">Identificador de la orden.</param>
    /// <param name="userId">Identificador del usuario.</param>
    /// <returns>Detalle de la orden.</returns>
    Task<GenericResponse<OrderDTO>> GetOrderDetailByIdAsync(int orderId, int userId);

    /// <summary>
    /// Obtiene todas las órdenes con filtros (solo admin).
    /// </summary>
    /// <param name="filter">Filtros de búsqueda.</param>
    /// <returns>Lista paginada de órdenes.</returns>
    Task<GenericResponse<OrderListDTO>> GetAllOrdersAsync(OrderFilterDTO filter);

    /// <summary>
    /// Obtiene el detalle de cualquier orden por su ID (solo admin).
    /// </summary>
    /// <param name="orderId">Identificador de la orden.</param>
    /// <returns>Detalle de la orden.</returns>
    Task<GenericResponse<OrderDTO>> GetOrderByIdAsync(int orderId);

    /// <summary>
    /// Actualiza el estado de una orden (solo admin).
    /// </summary>
    /// <param name="orderId">Identificador de la orden.</param>
    /// <param name="dto">DTO con el nuevo estado.</param>
    /// <param name="adminId">Identificador del administrador que realiza el cambio.</param>
    /// <returns>Orden actualizada.</returns>
    Task<GenericResponse<OrderDTO>> UpdateOrderStatusAsync(
        int orderId,
        UpdateOrderStatusDTO dto,
        int adminId
    );
}
