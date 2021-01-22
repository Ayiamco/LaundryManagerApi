//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;
//using LaundryApi.Interfaces;
//using LaundryApi.Models;
//using LaundryApi.Dtos;
//using Microsoft.AspNetCore.Mvc;
//using static LaundryApi.Infrastructure.HelperMethods;
//using LaundryApi.Infrastructure;
//using AutoMapper;

//namespace LaundryApi.Repositories
//{
//    public class CustomerRespository:ControllerBase, ICustomerRepository
//    {
//        private readonly LaundryApiContext _context;
//        private readonly IMapper mapper;

//        public CustomerRespository(LaundryApiContext _context, IMapper mapper)
//        {
//            this._context = _context;
//            this.mapper = mapper;
//        }

//        public async Task<CustomerDto> AddCustomer(CustomerDto customerDto,string username)
//        {
//            try
//            {
//                Laundry laundry = _context.Laundries.FirstOrDefault(u => u.Username == username);
//                var customer = mapper.Map<Customer>(customerDto);
//                customer.CreatedAt = DateTime.Now;
//                customer.LaundryId = laundry.LaundryId;
//                customer.TotalPurchase = 0;
//                await _context.Customers.AddAsync(customer);
                
//                //update laundry object
//                laundry.NoOfCustomers++;
//                Laundry laundryInDb = _context.Laundries.Single(x => x.LaundryId == laundry.LaundryId);
//                mapper.Map(laundry,laundryInDb);

//                await _context.SaveChangesAsync();
//                var returnObj = mapper.Map<CustomerDto>(customer);
//                return returnObj;
//            }
//            catch
//            {
//               throw new Exception(ErrorMessage.InvalidToken);
               
//            }

//        }

//        public async void UpdateCustomer(CustomerDto customerDto)
//        {
//            try
//            {
                
//                Customer customerInDb = _context.Customers.FirstOrDefault(x => x.Email == customerDto.Email);
//                if (customerInDb == null)
//                    throw new Exception(ErrorMessage.UserDoesNotExist);
//                mapper.Map(customerDto, customerInDb);
//                await _context.SaveChangesAsync();
//                return;
//            }
//            catch
//            {
//                throw new Exception(ErrorMessage.FailedDbOperation);
//            }
            
//        }
//        public async void UpdateTotalPurchase(Guid customerId,decimal amt)
//        {
//            try
//            {

//                Customer customerInDb = await _context.Customers.FindAsync(customerId);
//                if (customerInDb == null)
//                    throw new Exception(ErrorMessage.UserDoesNotExist);
//                customerInDb.TotalPurchase = customerInDb.TotalPurchase + amt;
//                await _context.SaveChangesAsync();
//                return;
//            }
//            catch
//            {
//                throw new Exception(ErrorMessage.FailedDbOperation);
//            }

//        }

//        public  void DeleteCustomer(Guid customerId)
//        {
//            try
//            {
//                var customer = _context.Customers.SingleOrDefault(c => c.CustomerId == customerId);
//                //_context.SaveChanges();
//                if (customer == null)
//                    throw new Exception(ErrorMessage.UserDoesNotExist);

//                _context.Customers.Attach(customer);
//                _context.Customers.Remove(customer);
//                _context.SaveChanges();
//                return;
//            }
//            catch 
//            {
//                throw new Exception(ErrorMessage.FailedDbOperation);
//            }
            
//        }

//        public  CustomerDto GetCustomer(Guid customerId)
//        {
//            var customer=  _context.Customers.FirstOrDefault(c => c.CustomerId == customerId);
//            if (customer == null)
//                throw new Exception(ErrorMessage.UserDoesNotExist);

//            var _customer = mapper.Map<CustomerDto>(customer);
//            return _customer;
//        }
//    }
//}
