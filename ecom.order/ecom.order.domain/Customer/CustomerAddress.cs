namespace ecom.order.domain.Customer
{
    public class CustomerAddress
    {
        public string? Address { get; set; } = string.Empty;
        public string? City { get; private set; } = string.Empty;
        public string? PostalCode { get; set; } = string.Empty;
        public string? Country { get; set; } = string.Empty;
    }
}
