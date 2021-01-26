using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LaundryApi.Interfaces;
using LaundryApi.Models;
using LaundryApi.Dtos;
using Microsoft.AspNetCore.Mvc;
using static LaundryApi.Infrastructure.HelperMethods;
using LaundryApi.Infrastructure;
using AutoMapper;

namespace LaundryApi.Repositories
{
    public class CustomerRespository : ControllerBase, ICustomerRepository
    {
        private readonly LaundryApiContext _context;
        private readonly IMapper mapper;

        public CustomerRespository(LaundryApiContext _context, IMapper mapper)
        {
            this._context = _context;
            this.mapper = mapper;
        }

        public async Task<CustomerDto> AddCustomer(CustomerDto customerDto, string username)
        {
            try
            { 
                //get application registering the customer
                Employee employeeInDb = _context.Employees.SingleOrDefault(u => u.Username == username);
                Laundry laundryInDb = _context.Laundries.SingleOrDefault(u => u.Username == username);
                
                //map the customerdto to the customer obj and update missing properties
                var customer = mapper.Map<Customer>(customerDto);
                customer.CreatedAt = DateTime.Now;
                customer.TotalPurchase = 0;
                customer.UpdatedAt = DateTime.Now;

                //attach the current user to  customer object
                if (employeeInDb != null)
                {
                    customer.EmployeeId = employeeInDb.Id;
                    customer.LaundryId = employeeInDb.LaundryId;
                    employeeInDb.NoOfCustomers++; //updating employee object
                    laundryInDb.NoOfCustomers++; //updating laundry object
                }
                else
                {
                    customer.LaundryId = laundryInDb.Id;
                    laundryInDb.NoOfCustomers++; //updating laundry object
                }   
             
                //add the customer to the db context
                await _context.Customers.AddAsync(customer);

                //save changes
                await _context.SaveChangesAsync();

                //return customer dto
                var returnObj = mapper.Map<CustomerDto>(customer);
                return returnObj;
            }
            catch
            {
                throw new Exception(ErrorMessage.FailedDbOperation);

            }

        }

        public  void UpdateCustomer(CustomerDto customerDto)
        {
            try
            {
                //get the customer
                Customer customerInDb = _context.Customers.FirstOrDefault(x => x.Username == customerDto.Username);

                //validate that the customer exist 
                if (customerInDb == null)
                    throw new Exception(ErrorMessage.EntityDoesNotExist);

                //update the changes
                customerInDb.Address = customerDto.Address;
                customerInDb.Name = customerDto.Name;
                customerInDb.PhoneNumber = customerDto.PhoneNumber;
                customerInDb.UpdatedAt = DateTime.Now;

                //save changes
                 _context.SaveChanges();

                return;
            }
            catch(Exception e)
            {
                if (e.Message == ErrorMessage.EntityDoesNotExist)
                    throw new Exception(ErrorMessage.EntityDoesNotExist);

                throw new Exception(ErrorMessage.FailedDbOperation);
            }

        }
        public async void UpdateTotalPurchase(Guid customerId, decimal amt)
        {
            try
            {
                Customer customerInDb = await _context.Customers.FindAsync(customerId);
                if (customerInDb == null)
                    throw new Exception(ErrorMessage.UserDoesNotExist);
                customerInDb.TotalPurchase = customerInDb.TotalPurchase + amt;
                await _context.SaveChangesAsync();
                return;
            }
            catch
            {
                throw new Exception(ErrorMessage.FailedDbOperation);
            }

        }

        public void DeleteCustomer(Guid customerId)
        {
            try
            {
                //get the customer into the context
                Customer customerInDb = _context.Customers.SingleOrDefault(c => c.Id == customerId);
                
                //check if the customer exist in db
                if (customerInDb == null)
                    throw new Exception(ErrorMessage.EntityDoesNotExist);

                //tag the customer as deleted
                customerInDb.IsDeleted = true;

                //save changes
                _context.SaveChanges();
                return;
            }
            catch(Exception e)
            {
                if (e.Message == ErrorMessage.EntityDoesNotExist)
                    throw new Exception(ErrorMessage.EntityDoesNotExist);

                throw new Exception(ErrorMessage.FailedDbOperation);
            }

        }

        public CustomerDto GetCustomer(Guid customerId)
        {
            try
            {
                var customer = _context.Customers.SingleOrDefault(c => c.Id == customerId);
                if (customer == null)
                    throw new Exception(ErrorMessage.EntityDoesNotExist);

                var _customer = mapper.Map<CustomerDto>(customer);
                return _customer;
            }
            catch(Exception e)
            {
                if (e.Message == ErrorMessage.EntityDoesNotExist)
                    throw new Exception(ErrorMessage.EntityDoesNotExist);

                throw new Exception(ErrorMessage.FailedDbOperation);
            }
            
        }
    }
}
