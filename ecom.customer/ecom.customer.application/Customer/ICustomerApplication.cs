using ecom.customer.domain.Customer;

namespace ecom.customer.application.Customer
{
    public interface ICustomerApplication
    {
        Task<CustomerDetails> GetAsync(string id);
        Task<IEnumerable<CustomerDetails>> ListAsync();
        Task<string> AddAsync(CustomerDetails product);
        Task<string> UpdateAsync(CustomerDetails customer);
    }
}
