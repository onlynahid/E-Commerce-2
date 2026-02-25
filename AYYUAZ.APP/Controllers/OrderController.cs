using AYYUAZ.APP.Application.Dtos;
using AYYUAZ.APP.Application.Interfaces;
using AYYUAZ.APP.Attributes;
using AYYUAZ.APP.Domain.Entities;
using AYYUAZ.APP.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Mvc;

namespace AYYUAZ.APP.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;
        //private readonly IBasketService _basketService;
        private readonly AppDbContext _context;
        public OrderController(IOrderService orderService,/* IBasketService basketService*/ AppDbContext context)
        {
            _orderService = orderService;
            _context = context;

        }
        [HttpGet("{id}")]
        public async Task<ActionResult<OrderDto>> GetOrderById(int id)
        {
            var order = await _orderService.GetOrderByIdAsync(id);
            if (order == null)
            {
                return NotFound($"Order with ID {id} not found.");
            }
            return Ok(order);
        }
        
        [HttpPost]
  
        public async Task<ActionResult<OrderDto>> CreateOrder([FromBody] CreateOrderDto createOrderDto)
        {
           
            try
            {
                var order = await _orderService.CreateOrderAsync(createOrderDto);
                return CreatedAtAction(nameof(GetOrderById), new { id = order.Id }, order);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        
        [HttpPut("{id}")]
        public async Task<ActionResult<OrderDto>> UpdateOrder(int id, [FromBody] UpdateOrderDto updateOrderDto)
        {
            if (id != updateOrderDto.Id)
            {
                return BadRequest("Order ID mismatch.");
            }
                var order = await _orderService.UpdateOrderAsync(updateOrderDto);
                if (order == null)
                {
                    return NotFound($"Order with ID {id} not found.");
                }
                return Ok(order);
        }
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteOrder(int id)
        {
            var result = await _orderService.DeleteOrderAsync(id);
            if (!result)
            {
                return NotFound($"Order with ID {id} not found.");
            }
            return NoContent();
        }
        
        [HttpPost("checkout")]
        public async Task<IActionResult> Checkout(CreateOrderDto dto)
        {
           if(dto.OrderItems==null || !dto.OrderItems.Any())
                return BadRequest("No items in the order.");

            decimal total = 0;
            var orderItems = new List<OrderItem>();

            foreach (var item in dto.OrderItems)
            {
                var product = await _context.Products.FindAsync(item.ProductId);
                if (product == null)
                {
                    return BadRequest($"Product with ID {item.ProductId} not found.");
                }
                
                if (item.Quantity <= 0)
                {
                    return BadRequest("Quantity must be greater than zero.");
                }
                
                total += product.Price * item.Quantity;
                
                // OrderItem yarat
                orderItems.Add(new OrderItem
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    UnitPrice = product.Price
                });
            }
            
            var order = new Order
            {
                FullName = dto.FullName,
                Email = dto.Email,
                Address = dto.Address,
                PhoneNumber = dto.PhoneNumber,
                Notes = dto.Notes,
                CreatedAt = DateTime.UtcNow,
                TotalAmount = total,
                OrderStatus = Domain.Enum.OrderStatus.Processed,
                OrderItems = orderItems  
            };
            
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();
            
            return Ok(new { 
                OrderId = order.Id,
                TotalAmount = total,
                ItemCount = orderItems.Count
            });
        }
    }
}



