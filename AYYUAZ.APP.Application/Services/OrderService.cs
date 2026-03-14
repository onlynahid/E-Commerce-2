using AYYUAZ.APP.Application.Dtos;
using AYYUAZ.APP.Application.Interfaces;
using AYYUAZ.APP.Domain.Entities;
using AYYUAZ.APP.Domain.Interfaces;
using AYYUAZ.APP.Domain.Enum;
using AYYUAZ.APP.Application.Exceptions.AppException;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AYYUAZ.APP.Constants;

namespace AYYUAZ.APP.Application.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IProductRepository _productRepository;
        public OrderService(IOrderRepository orderRepository, IProductRepository productRepository)
        {
            _orderRepository = orderRepository;
            _productRepository = productRepository;
        }
        public async Task<OrderAcceptedDto> AcceptedOrderAsync(int orderId)
        {
            var order = await _orderRepository.GetById(orderId);
            if (order == null)
                throw new NotFoundException(ErrorMessages.OrderNotFound);

            if (order.OrderStatus == OrderStatus.Accepted)
                throw new ConflictException(ErrorMessages.OrderAlreadyAccepted);

            if (order.OrderStatus == OrderStatus.Rejected)
                throw new ConflictException(ErrorMessages.OrderAlreadyRejected);

            order.OrderStatus = OrderStatus.Accepted;
            order.AcceptedAt = DateTime.UtcNow;

            await _orderRepository.UpdateAsync(order);

            return new OrderAcceptedDto
            {
                AcceptedAt = order.AcceptedAt,
                Status = OrderStatus.Accepted,
                OrderItems = order.OrderItems.Select(oi => new OrderItemDto
                {
                    Id = oi.Id,
                    OrderId = oi.OrderId,
                    ProductId = oi.ProductId,
                    ProductName = oi.Product?.Name ?? string.Empty,
                    ProductImageUrl = oi.Product?.ImageUrl ?? string.Empty,
                    Quantity = oi.Quantity,
                    UnitPrice = oi.UnitPrice
                }).ToList()
            };
        }
        public async Task<OrderDto> RejectedOrderAsync(int orderId, string? rejectedReason = null)
        {
            var order = await _orderRepository.GetById(orderId);
            if (order == null)
                throw new NotFoundException(ErrorMessages.OrderNotFound);

            if (order.OrderStatus == OrderStatus.Rejected)
                throw new ConflictException(ErrorMessages.OrderAlreadyRejected);

            if (order.OrderStatus == OrderStatus.Accepted)
                throw new ConflictException(ErrorMessages.OrderAlreadyAccepted);

            order.OrderStatus = OrderStatus.Rejected;
            order.RejectedAt = DateTime.UtcNow;
            order.RejectedReason = string.IsNullOrWhiteSpace(rejectedReason)
                ? "Order rejected"
                : rejectedReason.Trim();

            await _orderRepository.UpdateAsync(order);

            return MapToDto(order);
        }
        public async Task<OrderDto> RejectedOrderDto(int orderId)
        {
            return await RejectedOrderAsync(orderId, "Order rejected by administrator");
        }
        public async Task<OrderDto> CreateOrderAsync(CreateOrderDto createOrderDto)
        {
            var order = new Order
            {
                FullName = createOrderDto.FullName,
                PhoneNumber = createOrderDto.PhoneNumber,
                Email = createOrderDto.Email,
                Address = createOrderDto.Address,
                Notes = createOrderDto.Notes,
                CreatedAt = DateTime.UtcNow,
                OrderStatus = OrderStatus.Processed,
                TotalAmount = 0,
                AcceptedAt = null,
                RejectedAt = null,
                RejectedReason = null,
                OrderItems = new List<OrderItem>()
            };

            decimal calculatedTotal = 0;
            
            foreach (var itemDto in createOrderDto.OrderItems)
            {
                var product = await _productRepository.GetById(itemDto.ProductId);
                if (product == null)
                {
                    throw new NotFoundException();
                }

                var orderItem = new OrderItem
                {
                    ProductId = itemDto.ProductId,
                    Quantity = itemDto.Quantity,
                    UnitPrice = product.Price,
                    Order = order
                };
                
                order.OrderItems.Add(orderItem);
                calculatedTotal += orderItem.Quantity * orderItem.UnitPrice;
            }

            order.TotalAmount = calculatedTotal;

            await _orderRepository.AddAsync(order);
            return await GetOrderById(order.Id);
        }
        public async Task<bool> DeleteOrderAsync(int orderId)
        {
            var order = await _orderRepository.GetById(orderId);
            if (order == null)
                throw new NotFoundException(ErrorMessages.OrderNotFound);

            await _orderRepository.DeleteAsync(orderId);
            return true;
        }
        public Task<IEnumerable<OrderDto>> GetAllOrders()
        {
            return _orderRepository.GetAll().ContinueWith(task =>
            {
                var orders = task.Result;
                return orders.Select(MapToDto);
            });
        }
        public async Task<decimal> GetAverageOrderValue()
        {
            var orders = await _orderRepository.GetAll();
            if (!orders.Any()) 
                return 0;

            return orders.Average(o => o.TotalAmount);
        }
        public Task<OrderDto> GetOrderById(int orderId)
        {
            return _orderRepository.GetById(orderId).ContinueWith(task =>
            {
                var order = task.Result;
                return order == null ? throw new NotFoundException(ErrorMessages.OrderNotFound) : MapToDto(order);
            });
        }
        public async Task<int> GetOrderCount()
        {
            var orders = await _orderRepository.GetAll();
            return orders.Count();
        }
        public async Task<IEnumerable<OrderDto>> GetOrdersByCustomer(string customerName)
        {
            var orders = await _orderRepository.GetAll();
            return orders
                .Where(o => o.FullName.Contains(customerName, StringComparison.OrdinalIgnoreCase))
                .Select(MapToDto);
        }
        public async Task<IEnumerable<OrderDto>> GetOrdersByDateRange(DateTime startDate, DateTime endDate)
        {
            var orders = await _orderRepository.GetAll();
            return orders
                .Where(o => o.CreatedAt >= startDate && o.CreatedAt <= endDate)
                .Select(MapToDto);
        }
        public async Task<IEnumerable<OrderDto>> GetOrdersWithItems()
        {
            var orders = await _orderRepository.GetAllWithItems();
            return orders
                .Where(o => o.OrderItems != null && o.OrderItems.Any())
                .Select(MapToDtoWithItems);
        }
        public async Task<IEnumerable<OrderDto>> GetOrdersWithPagination(int page, int pageSize)
        {
            var orders = await _orderRepository.GetAll();
            return orders
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(MapToDto);
        }
        public async Task<OrderDto> GetOrderWithItems(int orderId)
        {
            var order = await _orderRepository.GetByIdWithItems(orderId);
            return order == null ? throw new NotFoundException(ErrorMessages.OrderNotFound) : MapToDtoWithItems(order);
        }
        public async Task<IEnumerable<OrderDto>> GetRecentOrders(int count = 10)
        {
            var orders = await _orderRepository.GetAll();
            return orders
                .OrderByDescending(o => o.CreatedAt)
                .Take(count)
                .Select(MapToDto);
        }
        public async Task<IEnumerable<OrderDto>> SearchOrders(string searchTerm)
        {
            var orders = await _orderRepository.GetAll();
            return orders
                .Where(o => o.FullName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                           o.PhoneNumber.Contains(searchTerm) ||
                           (o.Email != null && o.Email.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                           o.Address.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                .Select(MapToDto);
        }
        public async Task<OrderDto> UpdateOrderAsync(UpdateOrderDto updateOrderDto)
        {
            var order = await _orderRepository.GetById(updateOrderDto.Id);
            if (order == null)
                throw new NotFoundException(ErrorMessages.OrderNotFound);

            order.FullName = updateOrderDto.FullName ?? order.FullName;
            order.PhoneNumber = updateOrderDto.PhoneNumber ?? order.PhoneNumber;
            order.Email = updateOrderDto.Email ?? order.Email;
            order.Address = updateOrderDto.Address ?? order.Address;
            order.Notes = updateOrderDto.Notes ?? order.Notes;

            await _orderRepository.UpdateAsync(order);
            return MapToDto(order);
        }
        private OrderDto MapToDto(Order order)
        {
            return new OrderDto
            {
                Id = order.Id,
                FullName = order.FullName,
                PhoneNumber = order.PhoneNumber,
                Email = order.Email,
                Address = order.Address,
                Notes = order.Notes,
                CreatedAt = order.CreatedAt,
                TotalAmount = order.TotalAmount,
                OrderStatus = order.OrderStatus,
                AcceptedAt = order.AcceptedAt != default(DateTime) ? order.AcceptedAt : null,
                RejectedAt = order.RejectedAt != default(DateTime) ? order.RejectedAt : null,
                RejectedReason = order.RejectedReason,
                OrderItems = order.OrderItems?.Select(oi => new OrderItemDto
                {
                    Id = oi.Id,
                    OrderId = oi.OrderId,
                    ProductId = oi.ProductId,
                    ProductName = oi.Product?.Name ?? string.Empty,
                    ProductImageUrl = oi.Product?.ImageUrl ?? string.Empty,
                    Quantity = oi.Quantity,
                    UnitPrice = oi.UnitPrice,
                   
                }).ToList() ?? new List<OrderItemDto>()
            };
        }
        private OrderDto MapToDtoWithItems(Order order)
        {
            return new OrderDto
            {
                Id = order.Id,
                FullName = order.FullName,
                PhoneNumber = order.PhoneNumber,
                Email = order.Email,
                Address = order.Address,
                Notes = order.Notes,
                CreatedAt = order.CreatedAt,
                TotalAmount = order.TotalAmount,
                OrderStatus = order.OrderStatus,
                AcceptedAt = order.AcceptedAt != default(DateTime) ? order.AcceptedAt : null,
                RejectedAt = order.RejectedAt != default(DateTime) ? order.RejectedAt : null,
                RejectedReason = order.RejectedReason,
                OrderItems = order.OrderItems?.Select(oi => new OrderItemDto
                {
                    Id = oi.Id,
                    OrderId = oi.OrderId,
                    ProductId = oi.ProductId,
                    ProductName = oi.Product?.Name ?? "",
                    ProductImageUrl = oi.Product?.ImageUrl ?? "",
                    Quantity = oi.Quantity,
                    UnitPrice = oi.UnitPrice
                }).ToList() ?? new List<OrderItemDto>()
            };
        }
    }
}
