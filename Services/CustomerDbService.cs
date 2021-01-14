using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LaundryApi.Interfaces;
using LaundryApi.Models;
using LaundryApi.Dtos;
using Microsoft.AspNetCore.Mvc;
using static LaundryApi.Services.HelperMethods;
using AutoMapper;

namespace LaundryApi.Services
{
    public class CustomerDbService:ControllerBase, ICustomerDbService
    {
        private readonly LaundryApiContext _context;
        private readonly IMapper mapper;

        public CustomerDbService(LaundryApiContext _context, IMapper mapper)
        {
            this._context = _context;
            this.mapper = mapper;
        }

        public async Task<Customer> AddCustomer(CustomerDto customerDto,string username)
        {
            try
            {
                Laundry laundry = _context.Laundries.FirstOrDefault(u => u.Username == username);
                var customer = mapper.Map<Customer>(customerDto);
                customer.CreatedAt = DateTime.Now;
                customer.LaundryId = laundry.LaundryId;
                customer.TotalPurchase = 0;
                await _context.Customers.AddAsync(customer);
                
                //update laundry object
                laundry.NoOfCustomers++;
                Laundry laundryInDb = _context.Laundries.Single(x => x.LaundryId == laundry.LaundryId);
                mapper.Map(laundry,laundryInDb);

                await _context.SaveChangesAsync();
                return customer;
            }
            catch
            {
               throw new Exception(ErrorMessage.InvalidToken);
               
            }

        }

        public async void UpdateCustomer(CustomerDto customerDto)
        {
            try
            {
                if (customerDto == null)
                    throw new Exception(ErrorMessage.UserDoesNotExist);
                Customer customerInDb = _context.Customers.Single(x => x.Email == customerDto.Email);
                mapper.Map(customerDto, customerInDb);
                await _context.SaveChangesAsync();
                return;
            }
            catch
            {
                throw new Exception(ErrorMessage.FailedDbOperation);
            }
            
        }

        public async void DeleteCustomer(string customerEmail)
        {
            try
            {
                var customer = _context.Customers.FirstOrDefault(c => c.Email == customerEmail);
                if (customer == null)
                    throw new Exception(ErrorMessage.UserDoesNotExist);
                _context.Customers.Remove(customer);
               await  _context.SaveChangesAsync();
                return;
            }
            catch 
            {
                throw new Exception(ErrorMessage.FailedDbOperation);
            }
            
        }

        public  CustomerDto GetCustomer(Guid customerId)
        {
            var customer=  _context.Customers.FirstOrDefault(c => c.CustomerId == customerId);
            if (customer == null)
                throw new Exception(ErrorMessage.UserDoesNotExist);

            var _customer = mapper.Map<CustomerDto>(customer);
            return _customer;
        }
    }
}
