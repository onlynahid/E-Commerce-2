using AYYUAZ.APP.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;
using AYYUAZ.APP.Domain.Enum;

namespace AYYUAZ.APP.Domain.Interfaces
{
    public interface IOrderRepository : IGenericRepository<Order>
    {
        // Order-specific methods that are not in the generic repository
        Task<Order?> GetById(int id);
        Task<IEnumerable<Order>> GetAll();
        Task<Order?> GetByIdWithItems(int orderId);
        Task<IEnumerable<Order>> GetAllWithItems();
        Task<Order> AcceptOrderAsync(int orderId);
        Task<Order> RejectOrderAsync(int orderId, string? reason = null);
        Task<IEnumerable<Order>> GetByCustomerName(string customerName);
        Task<IEnumerable<Order>> GetByDateRange(DateTime startDate, DateTime endDate);
        Task<IEnumerable<Order>> SearchOrdersAsync(string searchTerm);
        Task<decimal> GetTotalSales();
        Task<decimal> GetTotalSalesByDateRange(DateTime startDate, DateTime endDate);
        Task<decimal> GetAverageOrderValue(); 
        Task<IEnumerable<Order>> GetRecentOrders(int count = 10);
    }
}
