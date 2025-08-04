using ecom.notification.domain.Customer;

namespace ecom.notification.infrastructure.Services.Customer
{
    public interface ICustomerService
    {
        Task<CustomerDetails> GetCustomerAsync(string customerId);
    }
}
