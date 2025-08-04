using Dapr.Client;
using ecom.notification.domain.Notification;
using Microsoft.Extensions.Logging;

namespace ecom.notification.infrastructure.Services.Email
{
    public class EmailService:IEmailService
    {
        private readonly DaprClient _daprClient;
        private readonly ILogger<EmailService> _logger;

        public EmailService(DaprClient daprClient, ILogger<EmailService> logger)
        {
            this._daprClient = daprClient;
            this._logger = logger;
        }

        public async Task SendEmail(OrderForNotfication order)
        {
            if (order == null)
            {
                throw new ArgumentNullException(nameof(order));
            }

            if (order.CustomerDetails == null || order.OrderDetails == null || order.ProductDetails == null)
            {
                throw new ArgumentException("Order details, customer details, or product details are missing", nameof(order));
            }

            _logger.LogInformation($"Received a new order, orderid {order.OrderDetails.OrderId} for {order.CustomerDetails.Email}");

            var metadata = new Dictionary<string, string>
            {
                ["emailFrom"] = "gagan1983@gmail.com",
                ["emailTo"] = order.CustomerDetails.Email ?? throw new InvalidOperationException("Customer email is required"),
                ["subject"] = $"Thank you for your order - Order Id = {order.OrderDetails.OrderId}"
            };

            // Sanitize input data before constructing the email body
            var sanitizedName = System.Web.HttpUtility.HtmlEncode(order.CustomerDetails.Name);
            var sanitizedAddress = System.Web.HttpUtility.HtmlEncode($"{order.CustomerDetails.Address?.Address}, {order.CustomerDetails.Address?.City} - {order.CustomerDetails.Address?.PostalCode}, {order.CustomerDetails.Address?.Country}");
            var sanitizedProductName = System.Web.HttpUtility.HtmlEncode(order.ProductDetails.Name);
            var sanitizedProductDesc = System.Web.HttpUtility.HtmlEncode(order.ProductDetails.Description);

            var body = $"<h2>Hi {sanitizedName}</h2>"
                + $"<p>Your order (order id - {order.OrderDetails.OrderId}) has been received and will be delivered "
                + $"to {sanitizedAddress} within 2 days.</p>"
                + $"</br><p><b>Order Details:</b></p>"
                + $"</br>Product Name - {sanitizedProductName}"
                + $"</br>Product Desc - {sanitizedProductDesc}"
                + $"</br>Price - {order.OrderDetails.OrderPrice}"
                + $"</br>Quantity - {order.OrderDetails.ProductCount}"
                + $"</br>Thanks, </br>Team TekSpry";

            try
            {
                await _daprClient.InvokeBindingAsync("sendmail", "create", body, metadata);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send email for order {OrderId}", order.OrderDetails.OrderId);
                throw;
            }
            
        }
    }
}
