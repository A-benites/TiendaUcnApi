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
}
