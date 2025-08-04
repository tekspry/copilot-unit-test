using ecom.notification.domain.Customer;
using ecom.notification.infrastructure.Extensions;

namespace ecom.notification.infrastructure.Services.Customer
{
    public class CustomerService : ICustomerService
    {
        private readonly HttpClient client;
        public CustomerService(HttpClient client)
        {
            this.client = client;
        }
        public async Task<CustomerDetails> GetCustomerAsync(string customerId)
        {
            var response = await client.GetAsync($"customer/{customerId}");
            
            return await response.ReadContentAs<CustomerDetails>();
        }
    }
}
