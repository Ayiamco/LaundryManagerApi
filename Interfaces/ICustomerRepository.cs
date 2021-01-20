using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LaundryApi.Models;
using LaundryApi.Dtos;
namespace LaundryApi.Interfaces
{
    public interface ICustomerRepository
    {
        public  Task<CustomerDto> AddCustomer(CustomerDto newCustomer,string username);
        public  void DeleteCustomer(Guid customerId);
        public void UpdateCustomer(CustomerDto customerDto);
        public CustomerDto GetCustomer(Guid customerId);
        public void UpdateTotalPurchase(Guid customerId, decimal amt);
    }
}
