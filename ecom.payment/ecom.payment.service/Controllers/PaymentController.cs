using Dapr;
using Dapr.Client;
using ecom.payment.domain.Order;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace ecom.payment.service.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // Secure all endpoints by default
    public class PaymentController : ControllerBase
    {
        private readonly ILogger<PaymentController> logger;
        private readonly DaprClient _daprClient;

        public PaymentController(ILogger<PaymentController> logger, DaprClient daprClient)
        {
            this._daprClient = daprClient;
            this.logger = logger;
        }

        [HttpPost(Name = "SubmitOrder")]
        [Topic("orderpubsub", "orders")] // This is secured by Dapr components configuration
        [AllowAnonymous] // Allow Dapr to call this endpoint
        public async Task<IActionResult> Submit(Order order)
        {
            logger.LogInformation($"Payment service received for new order: {order.OrderId} message from orders topic");
            
            try
            {
                // Log order details
                logger.LogInformation($"Order Details --> Product: {order.ProductId}, Product Quantity: {order.ProductCount}, Price: {order.OrderPrice}");

                // Process payment (in a real app, this would integrate with a payment provider)
                // For security, payment processing should be done in a separate service with proper PCI compliance

                // Publish payment success event
                await _daprClient.PublishEventAsync("orderpubsub", "payments", order);
                logger.LogInformation($"Payment done and published to orderpubsub component and payments topic for: {order.OrderId}");

                return Ok();
            }
            catch (Exception ex)
            {
                logger.LogError($"Error processing payment for order {order.OrderId}: {ex.Message}");
                return BadRequest("Payment processing failed");
            }
        }
    }
}
