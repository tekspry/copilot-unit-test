using Dapr.Client;
using ecom.customer.domain.Customer;
using Microsoft.Extensions.Logging;

namespace ecom.customer.database.Customer
{
    public class CustomerRepository : ICustomerRepository
    {
        private List<CustomerDetails> customers = new List<CustomerDetails>();
        private readonly DaprClient daprClient;
        private readonly ILogger<CustomerRepository> logger;
        private const string cacheStoreName = "customercache";

        public CustomerRepository(DaprClient daprClient, ILogger<CustomerRepository> logger)
        {
            this.daprClient = daprClient;
            this.logger = logger;

            LoadSampleData();
        }
        public async Task<string> CreateCustomer(CustomerDetails customer)
        {
            customer.Id = Guid.NewGuid().ToString();
            var key = $"customerlist";
            var customers = await daprClient.GetStateAsync<List<CustomerDetails>>(cacheStoreName, "customerlist");

            customers.Add(customer);
            await this.SaveCustomerToCacheStore(customers);

            return await Task.FromResult(customer.Id);
        }

        public async Task<CustomerDetails> GetCustomerById(string customerId)
        {
            var customers = await daprClient.GetStateAsync<List<CustomerDetails>>(cacheStoreName, "customerlist");
            var customer = customers.FirstOrDefault(e => e.Id == customerId);
            if (customer == null)
            {
                throw new InvalidOperationException("Event not found");
            }
            return customer;
        }

        public async Task<IEnumerable<CustomerDetails>> GetCustomers()
        {
            var customerList = await daprClient.GetStateAsync<List<CustomerDetails>>(cacheStoreName, "customerlist");

            return customerList;
        }

        public async Task<string> UpdateCustomer(CustomerDetails customer)
        {
            var key = $"customerlist";
            var customers = await daprClient.GetStateAsync<List<CustomerDetails>>(cacheStoreName, "customerlist");

            var updatedProduct = customers.Where(x => x.Id == customer.Id).FirstOrDefault();

            if (updatedProduct != null)
            {
                customers.Remove(updatedProduct);

                customers.Add(customer);
            }

            await this.SaveCustomerToCacheStore(customers);

            return customer.Id;
        }

        private async void LoadSampleData()
        {
            var Customer1Guid = Guid.Parse("{0787e3c6-5886-4c73-bbef-81f5bf79967b}").ToString();
            var Customer2Guid = Guid.Parse("{4599b039-bade-48dd-8459-f1bba3df4ccd}").ToString();
            var Customer3Guid = Guid.Parse("{9add0fa4-2414-43a7-af5e-b1a4ebbfcec0}").ToString();

            customers.Add(new CustomerDetails
            {
                Id = Customer1Guid,
                Name = "TestUser1",
                Email = "gagan1983@gmail.com",
                ContactNumber = 0111111111,
                Address = new CustomerAddress("test addr 1", "New Delhi", "123xyz", "India"),
                //Payments = new CustomerPaymentDetails()

            });

            customers.Add(new CustomerDetails
            {
                Id = Customer2Guid,
                Name = "TestUser2",
                Email = "gagan1983@gmail.com",
                ContactNumber = 0111111111,
                Address = new CustomerAddress("test addr 2", "New Delhi", "345xyz", "India"),
                //Payments = new CustomerPaymentDetails()

            });

            customers.Add(new CustomerDetails
            {
                Id = Customer3Guid,
                Name = "TestUser3",
                Email = "gagan1983@gmail.com",
                ContactNumber = 0111111111,
                Address = new CustomerAddress("test addr 3", "New Delhi", "345xyz", "India"),
                //Payments = new CustomerPaymentDetails()

            });

           

            await SaveCustomerToCacheStore(customers);
        }

        private async Task SaveCustomerToCacheStore(List<CustomerDetails> customers)
        {
            var key = $"customerlist";
            await daprClient.SaveStateAsync(cacheStoreName, key, customers);
            logger.LogInformation($"Created new customer in cache store {key}");
        }
    }
}
