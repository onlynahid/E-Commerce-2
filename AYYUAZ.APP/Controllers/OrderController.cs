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
        private readonly ILogger<OrderController> _logger;

        public OrderController(IOrderService orderService, ILogger<OrderController> logger)
        {
            _orderService = orderService;
            _logger = logger;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<OrderDto>> GetOrderById(int id)
        {
            _logger.LogInformation("Getting order by ID: {OrderId}", id);
            
            var order = await _orderService.GetOrderById(id);
            return Ok(order);
        }
        
        [HttpPost]
        public async Task<ActionResult<OrderDto>> CreateOrder([FromBody] CreateOrderDto createOrderDto)
        {
            _logger.LogInformation("Creating new order");
            
            var order = await _orderService.CreateOrderAsync(createOrderDto);
            return CreatedAtAction(nameof(GetOrderById), new { id = order.Id }, order);
        }
        
        [HttpPut("{id}")]
        public async Task<ActionResult<OrderDto>> UpdateOrder(int id, [FromBody] UpdateOrderDto updateOrderDto)
        {
            _logger.LogInformation("Updating order ID: {OrderId}", id);
            if (id <= 0)
            {
                return BadRequest("Invalid Order ID.");
            }
            await _orderService.UpdateOrderAsync(updateOrderDto,id); 
            var updatedOrder = await _orderService.GetOrderById(id);
            return Ok(updatedOrder);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteOrder(int id)
        {
            _logger.LogInformation("Deleting order ID: {OrderId}", id);
            
            await _orderService.DeleteOrderAsync(id);
            return NoContent();
        }
        
        [HttpPost("checkout")]
        public async Task<IActionResult> Checkout(CreateOrderDto dto)
        {
            _logger.LogInformation("Processing checkout");
           
            var order = await _orderService.CreateOrderAsync(dto);
            
            return Ok(new { 
                OrderId = order.Id,
                TotalAmount = order.TotalAmount,
                ItemCount = order.OrderItems?.Count ?? 0,
                Message = "Order created successfully"
            });
        }
    }
}



