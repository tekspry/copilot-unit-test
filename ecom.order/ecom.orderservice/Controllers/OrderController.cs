using ecom.order.application.Order;
using ecom.order.domain.Order;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace ecom.order.service.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize]
    public class OrderController : ControllerBase
    {
        private readonly IOrderApplication _orderApplication;
        private readonly ILogger<OrderController> _logger;

        public OrderController(IOrderApplication orderApplication, ILogger<OrderController> logger)
        {
            _orderApplication = orderApplication;
            _logger = logger;
        }

        [HttpPost(Name = "SubmitOrder")]
        public async Task<ActionResult<Order>> Submit(Order order)
        {
            _logger.LogInformation($"Submitting order: {order.OrderId}");
            try
            {
                var result = await _orderApplication.AddAsync(order);
                return CreatedAtAction(nameof(GetById), new { id = result.OrderId }, result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error submitting order: {ex.Message}");
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{id}", Name = "GetById")]
        public async Task<ActionResult<Order>> GetById(string id)
        {
            _logger.LogInformation($"Getting order by id: {id}");
            var order = await _orderApplication.GetAsync(id);
            if (order == null)
            {
                return NotFound();
            }
            return order;
        }

        [HttpGet(Name = "GetOrders")]
        public async Task<ActionResult<IEnumerable<OrderDetails>>> GetAll()
        {
            _logger.LogInformation("Getting all orders");
            return Ok(await _orderApplication.ListOrderDetailsAsync());
        }

        [HttpPut(Name = "UpdateOrder")]
        public async Task<IActionResult> Update(Order order)
        {
            _logger.LogInformation($"Updating order: {order.OrderId}");
            try
            {
                await _orderApplication.UpdateAsync(order);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error updating order: {ex.Message}");
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}", Name = "DeleteById")]
        public async Task<IActionResult> DeleteById(string id)
        {
            _logger.LogInformation($"Deleting order: {id}");
            try
            {
                await _orderApplication.DeleteAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error deleting order: {ex.Message}");
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete(Name = "DeleteAll")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteAll()
        {
            _logger.LogWarning("Deleting all orders - Admin only operation");
            try
            {
                await _orderApplication.DeleteAllAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error deleting all orders: {ex.Message}");
                return BadRequest(ex.Message);
            }
        }
    }
}
