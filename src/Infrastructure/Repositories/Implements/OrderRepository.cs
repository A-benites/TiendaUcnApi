using Microsoft.EntityFrameworkCore;
using TiendaUcnApi.src.Application.DTO.OrderDTO;
using TiendaUcnApi.src.Domain.Models;
using TiendaUcnApi.src.Infrastructure.Data;
using TiendaUcnApi.src.Infrastructure.Repositories.Interfaces;

namespace TiendaUcnApi.src.Infrastructure.Repositories.Implements;

public class OrderRepository : IOrderRepository
{
    private readonly AppDbContext _context;

    public OrderRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Order> CreateAsync(Order order)
    {
        _context.Orders.Add(order);
        await _context.SaveChangesAsync();
        return order;
    }

    public async Task<IEnumerable<Order>> GetAllByUser(int userId)
    {
        return await _context
            .Orders.Include(o => o.OrderItems)
            .Where(o => o.UserId == userId)
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync();
    }

    public async Task<(IEnumerable<Order> Orders, int TotalCount)> GetAllByUserPaginated(
        int userId,
        int page,
        int pageSize
    )
    {
        var query = _context.Orders.Include(o => o.OrderItems).Where(o => o.UserId == userId);

        // Get total count
        var totalCount = await query.CountAsync();

        // Apply pagination and ordering
        var orders = await query
            .OrderByDescending(o => o.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (orders, totalCount);
    }

    public async Task<Order?> GetByIdAsync(int id)
    {
        return await _context
            .Orders.Include(o => o.OrderItems)
            .Include(o => o.User)
            .FirstOrDefaultAsync(o => o.Id == id);
    }

    public async Task<(IEnumerable<Order> Orders, int TotalCount)> GetAllAsync(
        OrderFilterDTO filter
    )
    {
        var query = _context.Orders.Include(o => o.OrderItems).Include(o => o.User).AsQueryable();

        // Apply filters
        if (filter.Status.HasValue)
        {
            query = query.Where(o => o.Status == filter.Status.Value);
        }

        if (filter.UserId.HasValue)
        {
            query = query.Where(o => o.UserId == filter.UserId.Value);
        }

        if (filter.StartDate.HasValue)
        {
            query = query.Where(o => o.CreatedAt >= filter.StartDate.Value);
        }

        if (filter.EndDate.HasValue)
        {
            query = query.Where(o => o.CreatedAt <= filter.EndDate.Value);
        }

        if (!string.IsNullOrEmpty(filter.Code))
        {
            query = query.Where(o => o.Code.Contains(filter.Code));
        }

        // Get total count
        var totalCount = await query.CountAsync();

        // R116: Apply sorting based on SortBy parameter
        query = filter.SortBy switch
        {
            OrderSortOption.CreatedAtAsc => query.OrderBy(o => o.CreatedAt),
            OrderSortOption.TotalDesc => query.OrderByDescending(o => o.Total),
            OrderSortOption.TotalAsc => query.OrderBy(o => o.Total),
            OrderSortOption.CreatedAtDesc or _ => query.OrderByDescending(o => o.CreatedAt),
        };

        // Apply pagination
        var orders = await query
            .Skip((filter.Page - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .ToListAsync();

        return (orders, totalCount);
    }

    public async Task<Order> UpdateStatusAsync(int id, OrderStatus status)
    {
        var order = await _context.Orders.FindAsync(id);
        if (order == null)
        {
            throw new KeyNotFoundException($"Order with ID {id} not found.");
        }

        order.Status = status;
        order.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return order;
    }
}
