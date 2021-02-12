using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LaundryApi.Entites;
using LaundryApi.Dtos;
using LaundryApi.Models;

namespace LaundryApi.Interfaces
{
    public interface ICustomerRepository
    {
        public  Task<CustomerDto> AddCustomer(  NewCustomerDto newCustomer,string username,string userRole);
        public  Task<bool> DeleteCustomer(Guid customerId);
        public  Task<bool> UpdateCustomer(CustomerDto customerDto);
        public CustomerDto GetCustomer(Guid customerId);
        public IEnumerable<CustomerDto> GetCustomer(string customerName,string customerUsername);

        public PagedList<CustomerDto> GetCustomers(int pageSize, int pageNumber = 1);


        /// <summary>
        /// returns all the debtors in particular laundry
        /// </summary>
        public IEnumerable<CustomerDto> GetCustomer(string laundryUsername);

    }
}
