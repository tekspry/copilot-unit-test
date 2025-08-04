using ecom.customer.application.Customer;
using ecom.customer.domain.Customer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace ecom.customer.service.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerApplication _customerApplication;
        private readonly ILogger<CustomerController> _logger;

        public CustomerController(ICustomerApplication customerApplication, ILogger<CustomerController> logger)
        {
            _customerApplication = customerApplication;
            _logger = logger;
        }

        [HttpGet(Name = "GetCustomers")]
        public async Task<IEnumerable<CustomerDetails>> GetAll()
        {
            _logger.LogInformation("Getting all customers");
            return await _customerApplication.ListAsync();
        }

        [HttpGet("{id}", Name = "GetById")]
        public async Task<ActionResult<CustomerDetails>> GetById(string id)
        {
            _logger.LogInformation($"Getting customer by id: {id}");
            var customer = await _customerApplication.GetAsync(id);
            if (customer == null)
            {
                return NotFound();
            }
            return customer;
        }

        [HttpPost(Name = "AddCustomer")]
        public async Task<ActionResult<string>> Add(CustomerDetails customer)
        {
            _logger.LogInformation($"Adding new customer: {customer.Name}");
            try
            {
                var id = await _customerApplication.AddAsync(customer);
                return CreatedAtAction(nameof(GetById), new { id }, id);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error adding customer: {ex.Message}");
                return BadRequest(ex.Message);
            }
        }

        [HttpPut(Name = "UpdateCustomer")]
        public async Task<ActionResult<string>> Update(CustomerDetails customer)
        {
            _logger.LogInformation($"Updating customer: {customer.Id}");
            try
            {
                var id = await _customerApplication.UpdateAsync(customer);
                return Ok(id);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error updating customer: {ex.Message}");
                return BadRequest(ex.Message);
            }
        }
    }
}
