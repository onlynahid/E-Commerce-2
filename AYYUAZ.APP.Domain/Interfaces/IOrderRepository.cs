using AYYUAZ.APP.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace AYYUAZ.APP.Domain.Interfaces
{
    public interface IOrderRepository
    {                                      
        Task AddOrderAsync(Order order);
        Task UpdateOrderAsync(Order order);            
        Task DeleteOrderAsync(int orderId);
        Task<Order> GetOrderByIdAsync(int orderId);
        Task<IEnumerable<Order>> GetAllOrdersAsync();
        
        // Additional methods for including navigation properties
        Task<Order> GetOrderWithItemsAsync(int orderId);
        Task<IEnumerable<Order>> GetAllOrdersWithItemsAsync();
        Task<Order>AcceptedOrder(int orderId);
        Task<Order>RejectedOrder(int orderId);
    }
}
