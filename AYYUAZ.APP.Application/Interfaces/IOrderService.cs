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
        Task<IEnumerable<OrderDto>> GetAllOrdersAsync();
        Task<OrderDto> GetOrderByIdAsync(int orderId);
        Task<OrderDto> CreateOrderAsync(CreateOrderDto createOrderDto);
        Task<OrderDto> UpdateOrderAsync(UpdateOrderDto updateOrderDto);
        Task<bool> DeleteOrderAsync(int orderId);
        Task<OrderAcceptedDto> AcceptedOrderAsync(int orderId);
        Task<OrderDto> RejectedOrderDto(int orderId);
        Task<OrderDto> RejectedOrderAsync(int orderId, string rejectedReason = null);
        Task<IEnumerable<OrderDto>> GetOrdersByCustomerAsync(string customerName);
        Task<IEnumerable<OrderDto>> SearchOrdersAsync(string searchTerm);
        Task<IEnumerable<OrderDto>> GetOrdersWithPaginationAsync(int page, int pageSize);
        Task<int> GetOrderCountAsync();
        Task<IEnumerable<OrderDto>> GetRecentOrdersAsync(int count = 10);
        Task<decimal> GetAverageOrderValueAsync();
        Task<OrderDto> GetOrderWithItemsAsync(int orderId);
        Task<IEnumerable<OrderDto>> GetOrdersWithItemsAsync();
    }
}