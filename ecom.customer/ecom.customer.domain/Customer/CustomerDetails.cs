namespace ecom.customer.domain.Customer
{
    public class CustomerDetails
    {  
        public string? Id { get; set; }
        public string Name { get; set; } = String.Empty;
        public string Email { get; set; } = String.Empty;
        public int ContactNumber { get; set; }
        public CustomerAddress? Address { get; set; }
        //public CustomerPaymentDetails? Payments { get; set; }
    }
}
