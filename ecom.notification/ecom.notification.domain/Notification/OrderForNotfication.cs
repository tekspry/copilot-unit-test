namespace ecom.notification.domain.Notification
{
    public record OrderForNotfication(Order.Order OrderDetails, Customer.CustomerDetails CustomerDetails, Product.Product ProductDetails);    
}
