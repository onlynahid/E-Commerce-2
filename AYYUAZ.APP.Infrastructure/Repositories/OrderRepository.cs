using AYYUAZ.APP.Application.Exceptions.AppException;
using AYYUAZ.APP.Constants;
using AYYUAZ.APP.Domain.Entities;
using AYYUAZ.APP.Domain.Enum;
using AYYUAZ.APP.Domain.Interfaces;
using AYYUAZ.APP.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace AYYUAZ.APP.Infrastructure.Repositories
{
    public class OrderRepository : GenericRepository<Order>, IOrderRepository
    {
        public OrderRepository(AppDbContext context) : base(context)
        {
        }

        #region Override Generic Methods with Order-Specific Logic
        //public async Task<Order?> GetById(int id)
        //{
        //    return await GetByIdWithItems(id);
        //}

        //public async Task<IEnumerable<Order>> GetAll()
        //{
        //    return await GetAllWithItems();
        //}
        //public override async Task<Order> AddAsync(Order entity)
        //{
        //    entity.CreatedAt = DateTime.UtcNow;
        //    entity.OrderStatus = OrderStatus.Processed;
        //    if (entity.OrderItems?.Any() == true)
        //    {
        //        entity.TotalAmount = entity.OrderItems.Sum(oi => oi.Quantity * oi.UnitPrice);
        //    }

        //    await _dbSet.AddAsync(entity);
        //    await SaveChangesAsync();
        //    return entity;
        //}

        //public override async Task DeleteAsync(int id)
        //{
        //    var order = await GetByIdWithItems(id);
        //    if (order != null)
        //    {
        //        _dbSet.Remove(order);
        //        await SaveChangesAsync();
        //    }
        //}

        #endregion

        #region Order-Specific Methods

        public Task<Order?> GetByIdWithItems(int orderId)
        {
            return _dbSet
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .FirstOrDefaultAsync(o => o.Id == orderId);
        }

        public async Task<IEnumerable<Order>> GetAllWithItems()
        {
            return await _dbSet
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();
        }

        public async Task<Order> AcceptOrderAsync(int orderId)
        {
            var order = await _dbSet.FirstOrDefaultAsync(o => o.Id == orderId);
            if (order == null)
                throw new NotFoundException(ErrorMessages.OrderNotFound,orderId);

            if (order.OrderStatus == OrderStatus.Accepted)
                throw new ConflictException(ErrorMessages.OrderAlreadyAccepted);

            if (order.OrderStatus == OrderStatus.Rejected)
                throw new ConflictException(ErrorMessages.OrderAlreadyRejected);

            order.OrderStatus = OrderStatus.Accepted;
            order.AcceptedAt = DateTime.UtcNow;

            await SaveChangesAsync();
            return order;
        }

        public async Task<Order> RejectOrderAsync(int orderId, string? reason = null)
        {
            var order = await _dbSet.FirstOrDefaultAsync(o => o.Id == orderId);
            if (order == null)
                throw new NotFoundException(ErrorMessages.OrderNotFound);

            if (order.OrderStatus == OrderStatus.Rejected)
                throw new ConflictException(ErrorMessages.OrderAlreadyRejected);

            if (order.OrderStatus == OrderStatus.Accepted)
                throw new ConflictException(ErrorMessages.OrderAlreadyAccepted);

            order.OrderStatus = OrderStatus.Rejected;
            order.RejectedAt = DateTime.UtcNow;
            order.RejectedReason = string.IsNullOrWhiteSpace(reason) ? "Order rejected" : reason.Trim();

            await SaveChangesAsync();
            return order;
        }

        public async Task<IEnumerable<Order>> GetByStatusAsync(OrderStatus status)
        {
            return await _dbSet
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .Where(o => o.OrderStatus == status)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Order>> GetByCustomerName(string customerName)
        {
            if (string.IsNullOrWhiteSpace(customerName))
                return new List<Order>();

            return await _dbSet
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .Where(o => o.FullName.Contains(customerName))
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Order>> GetByDateRange(DateTime startDate, DateTime endDate)
        {
            return await _dbSet
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .Where(o => o.CreatedAt >= startDate && o.CreatedAt <= endDate)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Order>> SearchOrdersAsync(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return await GetAllWithItems();

            return await _dbSet
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .Where(o => o.FullName.Contains(searchTerm) ||
                           o.PhoneNumber.Contains(searchTerm) ||
                           (o.Email != null && o.Email.Contains(searchTerm)) ||
                           o.Address.Contains(searchTerm))
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();
        }

        public Task<decimal> GetTotalSales()
        {
            return _dbSet
                .Where(o => o.OrderStatus == OrderStatus.Accepted)
                .SumAsync(o => o.TotalAmount);
        }

        public Task<decimal> GetTotalSalesByDateRange(DateTime startDate, DateTime endDate)
        {
            return _dbSet
                .Where(o => o.OrderStatus == OrderStatus.Accepted &&
                           o.CreatedAt >= startDate &&
                           o.CreatedAt <= endDate)
                .SumAsync(o => o.TotalAmount);
        }

        public async Task<decimal> GetAverageOrderValue()
        {
            var orders = await _dbSet.Where(o => o.OrderStatus == OrderStatus.Accepted).ToListAsync();
            return orders.Any() ? orders.Average(o => o.TotalAmount) : 0;
        }

        public Task<int> GetOrderCountByStatus(OrderStatus status)
        {
            return _dbSet.CountAsync(o => o.OrderStatus == status);
        }

        public async Task<IEnumerable<Order>> GetRecentOrders(int count = 10)
        {
            return await _dbSet
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .OrderByDescending(o => o.CreatedAt)
                .Take(count)
                .ToListAsync();
        }

        #endregion
    }
}