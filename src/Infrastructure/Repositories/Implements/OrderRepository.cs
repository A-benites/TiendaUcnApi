using Microsoft.EntityFrameworkCore;
using TiendaUcnApi.src.Application.DTO.OrderDTO;
using TiendaUcnApi.src.Application.Exceptions;
using TiendaUcnApi.src.Application.Services;
using TiendaUcnApi.src.Domain.Models;
using TiendaUcnApi.src.Infrastructure.Data;
using TiendaUcnApi.src.Infrastructure.Repositories.Interfaces;

namespace TiendaUcnApi.src.Infrastructure.Repositories.Implements;

/// <summary>
/// Implementation of the order repository.
/// Handles database operations for orders including creation, retrieval, filtering, and status updates.
/// </summary>
public class OrderRepository : IOrderRepository
{
    private readonly AppDbContext _context;

    /// <summary>
    /// Initializes a new instance of the <see cref="OrderRepository"/> class.
    /// </summary>
    /// <param name="context">Database context.</param>
    public OrderRepository(AppDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Creates a new order.
    /// </summary>
    /// <param name="order">The order to create.</param>
    /// <returns>The created order.</returns>
    public async Task<Order> CreateAsync(Order order)
    {
        _context.Orders.Add(order);
        await _context.SaveChangesAsync();
        return order;
    }

    /// <summary>
    /// Retrieves all orders for a specific user.
    /// </summary>
    /// <param name="userId">The user ID.</param>
    /// <returns>Collection of orders ordered by creation date descending.</returns>
    public async Task<IEnumerable<Order>> GetAllByUser(int userId)
    {
        return await _context
            .Orders.Include(o => o.OrderItems)
            .Where(o => o.UserId == userId)
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync();
    }

    /// <summary>
    /// Retrieves paginated orders for a specific user.
    /// </summary>
    /// <param name="userId">The user ID.</param>
    /// <param name="page">Page number.</param>
    /// <param name="pageSize">Items per page.</param>
    /// <returns>Tuple containing the orders and total count.</returns>
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

    /// <summary>
    /// Retrieves an order by its ID with all related data.
    /// </summary>
    /// <param name="id">Order ID.</param>
    /// <returns>The order with items and user data, or null if not found.</returns>
    public async Task<Order?> GetByIdAsync(int id)
    {
        return await _context
            .Orders.Include(o => o.OrderItems)
            .Include(o => o.User)
            .FirstOrDefaultAsync(o => o.Id == id);
    }

    /// <summary>
    /// Retrieves all orders with filtering, sorting, and pagination.
    /// </summary>
    /// <param name="filter">Filter parameters including status, user, date range, code, sorting, and pagination.</param>
    /// <returns>Tuple containing filtered orders and total count.</returns>
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

    /// <summary>
    /// Updates the status of an order with validation and audit logging.
    /// Validates state transitions using a state machine and creates audit records.
    /// </summary>
    /// <param name="id">Order ID.</param>
    /// <param name="status">New order status.</param>
    /// <param name="adminId">ID of the administrator performing the status change.</param>
    /// <returns>The updated order.</returns>
    /// <exception cref="KeyNotFoundException">Thrown when the order is not found.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the status transition is invalid.</exception>
    public async Task<Order> UpdateStatusAsync(int id, OrderStatus status, int adminId)
    {
        var order = await _context.Orders.FindAsync(id);
        if (order == null)
        {
            throw new KeyNotFoundException($"Order with ID {id} not found.");
        }

        // R123: Validate state transition using state machine
        var previousStatus = order.Status;
        if (!OrderStatusTransitionValidator.IsValidTransition(previousStatus, status))
        {
            var errorMessage = OrderStatusTransitionValidator.GetTransitionErrorMessage(
                previousStatus,
                status
            );
            throw new InvalidOperationException(errorMessage);
        }

        // R125: Create audit record before changing status
        var auditRecord = new AuditRecord
        {
            ChangedById = adminId,
            TargetUserId = order.UserId,
            Action = "ChangeOrderStatus",
            PreviousValue = previousStatus.ToString(),
            NewValue = status.ToString(),
            Reason = $"Order {order.Code} status changed from {previousStatus} to {status}",
            ChangedAt = DateTime.UtcNow,
        };

        _context.AuditRecords.Add(auditRecord);

        // Update order status
        order.Status = status;
        order.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return order;
    }
}
