namespace ecom.order.infrastructure.Services.Customer
{
    public interface ICustomerService
    {
        Task<ecom.order.domain.Customer.Customer> GetCustomerAsync(string customerId);
    }
}
