using Dapr;
using ecom.notification.application.Notification;
using ecom.notification.domain.Order;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Text.Json;

namespace ecom.notification.service.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // Secure all endpoints by default
    public class NotificationController : ControllerBase
    {
        private readonly INotificationApplication _notificationApplication;
        private readonly ILogger<NotificationController> logger;

        public NotificationController(INotificationApplication notificationApplication, ILogger<NotificationController> logger)
        {
            _notificationApplication = notificationApplication;
            this.logger = logger;
        }

        [HttpPost(Name = "SubmitOrder")]
        [Topic("orderpubsub", "payments")] // This is secured by Dapr components configuration
        [AllowAnonymous] // Allow Dapr to call this endpoint
        public async Task<IActionResult> Submit([FromBody] Order order)
        {   
            if (order == null)
            {
                logger.LogWarning("Received null order in notification service");
                return BadRequest("Order cannot be null");
            }

            try
            {
                // Log safely - avoid logging sensitive data
                logger.LogInformation("Processing notification for Order ID: {OrderId}", order.OrderId);
                
                await _notificationApplication.SendNotificationAsync(order);
                
                logger.LogInformation("Successfully sent notification for Order ID: {OrderId}", order.OrderId);
                return Ok();
            }
            catch (Exception ex)
            {
                // Log error safely without exposing sensitive details
                logger.LogError(ex, "Error processing notification for Order ID: {OrderId}", order.OrderId);
                return StatusCode(500, "An error occurred while processing the notification");
            }
        }
    }
}
