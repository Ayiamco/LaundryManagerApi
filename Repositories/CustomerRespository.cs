using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LaundryApi.Interfaces;
using LaundryApi.Entites;
using LaundryApi.Dtos;
using Microsoft.AspNetCore.Mvc;
using static LaundryApi.Infrastructure.HelperMethods;
using LaundryApi.Infrastructure;
using LaundryApi.Models;
using AutoMapper;

namespace LaundryApi.Repositories
{
    public class CustomerRespository : ICustomerRepository
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

        public async Task<CustomerDto> AddCustomer(NewCustomerDto newCustomerDto, string username,string userRole)
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
                Customer customer = mapper.Map<Customer>(newCustomerDto);  
                customer.CreatedAt = DateTime.Now;
                customer.TotalPurchase = 0;
                customer.UpdatedAt = DateTime.Now;
                customer.Debt = 0;

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
                await _context.Customers.AddAsync(customer);
                await _context.SaveChangesAsync();
                var returnObj = mapper.Map<CustomerDto>(customer);
                return returnObj;
            }
            catch(Exception e)
            {
                if (e.InnerException.Message.Contains("Violation of UNIQUE KEY constraint 'AK_Customers_Username_LaundryId'"))
                    throw new Exception(ErrorMessage.UsernameAlreadyExist);
                throw new Exception(ErrorMessage.FailedDbOperation);

            }

        }

        public async Task<bool> UpdateCustomer(CustomerDto customerDto)
        {
            try
            {
                //get the customer
                Customer customerInDb =await _context.Customers.FindAsync(customerDto.Id);

                //validate that the customer exist 
                if (customerInDb == null)
                    throw new Exception(ErrorMessage.EntityDoesNotExist);

                //update the changes
                customerInDb.Address = customerDto.Address;
                customerInDb.Name = customerDto.Name;
                customerInDb.PhoneNumber = customerDto.PhoneNumber;
                customerInDb.UpdatedAt = DateTime.Now;

                //save changes
                await  _context.SaveChangesAsync();

                return true;
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

        public async Task<bool> DeleteCustomer(Guid customerId)
        {
            try
            {
                //get the customer into the context
                Customer customerInDb = await _context.Customers.FindAsync( customerId);
                
                //check if the customer exist in db
                if (customerInDb == null)
                    throw new Exception(ErrorMessage.EntityDoesNotExist);

                //tag the customer as deleted
                customerInDb.IsDeleted = true;

                //save changes
                await _context.SaveChangesAsync();
                return true;
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
                var customer = _context.Customers.SingleOrDefault(c => c.Id == customerId && c.IsDeleted);
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

        public IEnumerable<CustomerDto> GetCustomer(string customerName, string username)
        {
            IEnumerable<Customer> customers;
            if(string.IsNullOrWhiteSpace(username) && !string.IsNullOrWhiteSpace(customerName))
                customers= _context.Customers.Where(x => x.Name.Contains(customerName) && !x.IsDeleted);
            else if (string.IsNullOrWhiteSpace(customerName) && !string.IsNullOrWhiteSpace(username) )
                customers = _context.Customers.Where(x => x.Username.Contains(customerName) && !x.IsDeleted);
            else
                customers = _context.Customers.Where(x => x.Name.Contains(customerName) && x.Username.Contains(username) && !x.IsDeleted);

            if (customers.Count() == 0)//db search return nothing
                throw new Exception(ErrorMessage.NoEntityMatchesSearch);

            return mapper.Map<IEnumerable<CustomerDto>>(customers);
        }

        /// <summary>
        /// returns all the debtors in particular laundry
        /// </summary>
        public IEnumerable<CustomerDto> GetCustomer(string laundryUsername)
        {
            Laundry laundry=repositoryHelper.GetLaundryByUsername(laundryUsername);
            IEnumerable<Customer> debtors=_context.Customers.Where(x => x.LaundryId == laundry.Id && x.Debt > 0);
            if (debtors.Count() == 0)
                throw new Exception(ErrorMessage.NoEntityMatchesSearch);
            return mapper.Map<IEnumerable<CustomerDto>>(debtors);

        }

        public PagedList<CustomerDto> GetCustomers(int pageSize,int pageNumber=1)
        {
            var customerList = _context.Customers.ToList();
            var page = customerList.Skip((pageNumber - 1) * pageSize).Take(pageSize);

            PagedList<CustomerDto> obj = new PagedList<CustomerDto>()
            {
                Data = mapper.Map<IEnumerable<CustomerDto>>(page),
                PageNumber = pageNumber,
                PageSize = pageSize,
                OverallSize = customerList.Count,
            };

            return obj;
        }
    }

}
