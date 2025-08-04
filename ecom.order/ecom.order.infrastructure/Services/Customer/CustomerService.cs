using ecom.order.Extensions;

namespace ecom.order.infrastructure.Services.Customer
{
    public class CustomerService : ICustomerService
    {
        private readonly HttpClient client;
        public CustomerService(HttpClient client)
        {
            this.client = client;
        }
        public async Task<ecom.order.domain.Customer.Customer> GetCustomerAsync(string customerId)
        {
            var response = await client.GetAsync($"customer/{customerId}");
            var result = new ecom.order.domain.Customer.Customer();
            try
            {
                result = await response.ReadContentAs<ecom.order.domain.Customer.Customer>();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);

            }
            
            return result;
        }
    }
}
