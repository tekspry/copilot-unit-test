using ecom.notification.domain.Order;
using ecom.notification.infrastructure.Services.Customer;
using ecom.notification.infrastructure.Services.Product;
using Dapr.Client;
using Microsoft.Extensions.Logging;
using ecom.notification.domain.Notification;
using ecom.notification.infrastructure.Services.Email;

namespace ecom.notification.application.Notification
{
    public class NotificationApplication : INotificationApplication
    {
        private readonly IProductService _productService;
        private readonly ICustomerService _customerService;
        private readonly IEmailService _emailService;
        private readonly DaprClient _daprClient;
        private readonly ILogger<NotificationApplication> _logger;

        public NotificationApplication(IProductService productService, ICustomerService customerService, IEmailService emailService, DaprClient daprClient, ILogger<NotificationApplication> logger)
        {
            this._productService = productService;
            this._customerService = customerService;
            this._daprClient = daprClient;
            this._emailService = emailService;
            this._logger = logger;
        }
        public async Task SendNotificationAsync(Order order)
        {
            if (order == null)
            {
                throw new ArgumentNullException(nameof(order));
            }

            if (string.IsNullOrEmpty(order.CustomerId))
            {
                order.CustomerId = "0787e3c6-5886-4c73-bbef-81f5bf79967b";
            }

            if (string.IsNullOrEmpty(order.ProductId))
            {
                throw new ArgumentException("ProductId cannot be null or empty", nameof(order));
            }

            var customerDetails = await _customerService.GetCustomerAsync(order.CustomerId) 
                ?? throw new InvalidOperationException($"Customer not found for ID: {order.CustomerId}");
           
            var productDetails = await _productService.GetProductAsync(order.ProductId)
                ?? throw new InvalidOperationException($"Product not found for ID: {order.ProductId}");

            var orderForNotification = new OrderForNotfication(order, customerDetails, productDetails);
            
            await _emailService.SendEmail(orderForNotification);
        }
    }
}
