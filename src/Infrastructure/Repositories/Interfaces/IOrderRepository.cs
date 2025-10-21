
using TiendaUcnApi.src.Application.DTO.OrderDTO;
using TiendaUcnApi.src.Domain.Models;

namespace TiendaUcnApi.src.Infrastructure.Repositories.Interfaces;

public interface IOrderRepository
{
    Task<Order> CreateAsync(Order order);
    Task<IEnumerable<Order>> GetAllByUser(int userId);
    Task<Order?> GetByIdAsync(int id);
    Task<(IEnumerable<Order> Orders, int TotalCount)> GetAllAsync(OrderFilterDTO filter);
    Task<Order> UpdateStatusAsync(int id, OrderStatus status);
}
