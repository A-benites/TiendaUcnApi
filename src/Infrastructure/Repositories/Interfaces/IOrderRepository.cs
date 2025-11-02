using TiendaUcnApi.src.Application.DTO.OrderDTO;
using TiendaUcnApi.src.Domain.Models;

namespace TiendaUcnApi.src.Infrastructure.Repositories.Interfaces;

public interface IOrderRepository
{
    Task<Order> CreateAsync(Order order);
    Task<IEnumerable<Order>> GetAllByUser(int userId);
    Task<(IEnumerable<Order> Orders, int TotalCount)> GetAllByUserPaginated(
        int userId,
        int page,
        int pageSize
    );
    Task<Order?> GetByIdAsync(int id);
    Task<(IEnumerable<Order> Orders, int TotalCount)> GetAllAsync(OrderFilterDTO filter);
    Task<Order> UpdateStatusAsync(int id, OrderStatus status, int adminId);
}
