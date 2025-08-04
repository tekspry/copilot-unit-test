using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ecom.customer.domain.Customer
{
    public class CustomerAddress
    {
        public CustomerAddress(string? address, string? city, string? postalCode, string? country)
        {
            this.Address = address;
            this.City = city;
            this.PostalCode = postalCode;
            this.Country = country;
        }
        public string? Address { get; private set; } = string.Empty;
        public string? City { get; private set; } = string.Empty ;
        public string? PostalCode { get; private set; } = string.Empty;
        public string? Country { get; private set; } = string.Empty;
    }
}
