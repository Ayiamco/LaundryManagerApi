using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LaundryApi.Models;
using LaundryApi.Dtos;
namespace LaundryApi.Interfaces
{
    public interface ICustomerDbService
    {
        public  Task<Customer> AddCustomer(CustomerDto newCustomer,string username);
    }
}
