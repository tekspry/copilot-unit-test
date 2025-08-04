namespace ecom.order.domain.Order
{
    public class OrderDetails
    {
        public Order? order { get; set; }
        public Product.Product? ProductDetails { get; set; }
        public Customer.Customer? CustomerDetails { get; set; }       
    }

}
