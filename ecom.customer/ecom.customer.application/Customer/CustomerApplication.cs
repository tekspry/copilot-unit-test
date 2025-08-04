using ecom.customer.database.Customer;
using ecom.customer.domain.Customer;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ecom.customer.application.Customer
{
    public class CustomerApplication : ICustomerApplication
    {
        private readonly ICustomerRepository _customerRepository;

        private readonly ILogger<CustomerApplication> logger;

        public CustomerApplication(ICustomerRepository customerRepository, ILogger<CustomerApplication> logger)
        {
            _customerRepository = customerRepository;
            this.logger = logger;
        }


        public async Task<string> AddAsync(CustomerDetails customer) => await _customerRepository.CreateCustomer(customer);
       

        public async Task<CustomerDetails> GetAsync(string id) => await _customerRepository.GetCustomerById(id);
        

        public async Task<IEnumerable<CustomerDetails>> ListAsync() => await _customerRepository.GetCustomers();
       

        public async Task<string> UpdateAsync(CustomerDetails customer) => await _customerRepository.UpdateCustomer(customer);
        
    }
}
