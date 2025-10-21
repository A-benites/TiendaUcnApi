
using TiendaUcnApi.src.Domain.Models;

namespace TiendaUcnApi.src.Infrastructure.Repositories.Interfaces;

public interface IOrderRepository
{
    Task<Order> CreateAsync(Order order);
    Task<IEnumerable<Order>> GetAllByUser(int userId);
}
