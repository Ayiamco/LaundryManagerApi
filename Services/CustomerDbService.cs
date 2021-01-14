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
                await _context.Customers.AddAsync(customer);
                await _context.SaveChangesAsync();
                return customer;
            }
            catch
            {
               throw new Exception(ErrorMessage.InvalidToken);
               
            }

        }
    }
}
