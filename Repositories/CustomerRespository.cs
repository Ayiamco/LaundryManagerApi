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
        private readonly IRepositoryHelper repositoryHelper;
        public CustomerRespository(LaundryApiContext _context, IMapper mapper,IRepositoryHelper repositoryHelper)
        {
            this._context = _context;
            this.mapper = mapper;
            this.repositoryHelper = repositoryHelper;
        }

        public async Task<CustomerDto> AddCustomer(CustomerDto customerDto, string username,string userRole)
        {
            try
            {
                //get application user registering the customer
                Employee employee=null;
                Laundry laundry=null;
                if (userRole == RoleNames.LaundryOwner)
                    laundry = repositoryHelper.GetLaundryByUsername(username);
                else
                    employee = repositoryHelper.GetEmployeeByUsername(username);

                //map the customerdto to the customer obj and update missing properties
                Customer customer = mapper.Map<Customer>(customerDto);  
                customer.CreatedAt = DateTime.Now;
                customer.TotalPurchase = 0;
                customer.UpdatedAt = DateTime.Now;

                //assign the customer a laundryId and/or employeeId 
                if (userRole==RoleNames.LaundryEmployee && employee!=null)
                {
                    customer.EmployeeId = employee.Id;
                    customer.LaundryId = employee.LaundryId;

                    //updating employee object and laundry object
                    _context.Laundries.Find(laundry.Id).NoOfCustomers += 1;
                    _context.Employees.Find(employee.Id).NoOfCustomers += 1;
                }
                else
                {
                    customer.LaundryId = laundry.Id;

                    //updating the laundry object
                    _context.Laundries.Find(laundry.Id).NoOfCustomers += 1;
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
