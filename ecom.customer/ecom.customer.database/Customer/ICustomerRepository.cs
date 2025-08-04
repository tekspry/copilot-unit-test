using ecom.customer.domain.Customer;

namespace ecom.customer.database.Customer
{
    public interface ICustomerRepository
    {
        Task<string> CreateCustomer(CustomerDetails customer);
        Task<IEnumerable<CustomerDetails>> GetCustomers();
        Task<CustomerDetails> GetCustomerById(string customerId);        
        Task<string> UpdateCustomer(CustomerDetails customer);
    }
}
