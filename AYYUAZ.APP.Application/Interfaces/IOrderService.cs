using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AYYUAZ.APP.Application.Dtos;

namespace AYYUAZ.APP.Application.Interfaces
{
    public interface IOrderService 
    {    
        Task<IEnumerable<OrderDto>> GetAllOrders();
        Task<OrderDto> GetOrderById(int orderId);
        Task<OrderDto> CreateOrderAsync(CreateOrderDto createOrderDto);
        Task<OrderDto> UpdateOrderAsync(UpdateOrderDto updateOrderDto);
        Task<bool> DeleteOrderAsync(int orderId);
        Task<OrderAcceptedDto> AcceptedOrderAsync(int orderId);
        Task<OrderDto> RejectedOrderDto(int orderId);
        Task<OrderDto> RejectedOrderAsync(int orderId, string rejectedReason = null);
        Task<IEnumerable<OrderDto>> GetOrdersByCustomer(string customerName);
        Task<IEnumerable<OrderDto>> SearchOrders(string searchTerm);
        Task<IEnumerable<OrderDto>> GetOrdersWithPagination(int page, int pageSize);
        Task<int> GetOrderCount();
        Task<IEnumerable<OrderDto>> GetRecentOrders(int count = 10);
        Task<decimal> GetAverageOrderValue();
        Task<OrderDto> GetOrderWithItems(int orderId);
        Task<IEnumerable<OrderDto>> GetOrdersWithItems();
    }
}