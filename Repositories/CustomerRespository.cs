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
                Employee employee=null;
                Laundry laundry=null;
                if (userRole == RoleNames.LaundryOwner)
                    laundry = repositoryHelper.GetLaundryByUsername(username);
                else
                    employee = repositoryHelper.GetEmployeeByUsername(username);

                var customerInDb = laundry == null 
                    ?  _context.Customers.SingleOrDefault(x => x.LaundryId 
                       == employee.LaundryId && x.Username == newCustomerDto.Username)
                    : _context.Customers.SingleOrDefault(x => x.LaundryId
                        == laundry.Id && x.Username == newCustomerDto.Username);
                if (customerInDb != null)
                {
                    if (customerInDb.IsDeleted)
                    {
                        customerInDb.IsDeleted = false;
                        customerInDb.Name = newCustomerDto.Name;
                        customerInDb.PhoneNumber = newCustomerDto.PhoneNumber;
                        customerInDb.Address = newCustomerDto.Address;
                        await _context.SaveChangesAsync();
                    }
                    else
                        throw new Exception(ErrorMessage.UsernameAlreadyExist);
                }
                else
                    customerInDb= await AddCustomer(newCustomerDto, userRole, employee, laundry);

                var returnObj = mapper.Map<CustomerDto>(customerInDb);
                return returnObj;

            }
            catch(Exception e)
            {
                if (e.Message==ErrorMessage.UsernameAlreadyExist)
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
                customerInDb.Username = customerDto.Username;

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

        public void DeleteCustomer(Guid customerId)
        {
            try
            {
                Customer customerInDb = _context.Customers.Find( customerId);
                if (customerInDb == null)
                    throw new Exception(ErrorMessage.EntityDoesNotExist);

                customerInDb.IsDeleted = true;
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
                var customer = _context.Customers.SingleOrDefault(c => c.Id == customerId && c.IsDeleted==false);
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

        public IEnumerable<CustomerDto> GetCustomer(string laundryUsername)
        {
            Laundry laundry=repositoryHelper.GetLaundryByUsername(laundryUsername);
            IEnumerable<Customer> debtors=_context.Customers.Where(x => x.LaundryId == laundry.Id && x.Debt > 0);
            if (debtors.Count() == 0)
                throw new Exception(ErrorMessage.NoEntityMatchesSearch);
            return mapper.Map<IEnumerable<CustomerDto>>(debtors);

        }

        public PagedList<CustomerDto> GetPage(int pageSize, string laundryUsername, int pageNumber = 1, string searchParam = "")
        {
            var laundry = repositoryHelper.GetLaundryByUsername(laundryUsername);
            var customerList = _context.Customers.Where(x => x.IsDeleted == false && x.LaundryId == laundry.Id).ToList();
            if (searchParam != "")
            {
                customerList = customerList.Where(x => x.Name.Contains(searchParam.ToLower())).ToList();
                if(customerList.Count() / pageSize <= pageNumber)
                    pageNumber=  1;
            }
               
            var page = customerList.Skip((pageNumber - 1) * pageSize).Take(pageSize);
            var maxPage = customerList.Count / (decimal)pageSize;
            PagedList<CustomerDto> obj = new PagedList<CustomerDto>()
            {
                Data = mapper.Map<IEnumerable<CustomerDto>>(page),
                PageIndex = pageNumber,
                PageSize = pageSize,
            };
            if (maxPage < 1)
                obj.MaxPageIndex = 1;
            else
            {
                int _num;
                try
                {_num = Convert.ToInt32(Convert.ToString(maxPage).Split(".")[1]);}
                catch
                { _num = 0;}

                obj.MaxPageIndex = _num > 0 ? Convert.ToInt32(maxPage + 1) : Convert.ToInt32(maxPage);
            }
            return obj;
        }

        private async Task<Customer> AddCustomer(NewCustomerDto newCustomerDto,string userRole,Employee employee, Laundry laundry)
        {
            //map the customerdto to the customer obj and update missing properties
            Customer customer = mapper.Map<Customer>(newCustomerDto);
            customer.CreatedAt = DateTime.Now;
            customer.TotalPurchase = 0;
            customer.UpdatedAt = DateTime.Now;
            customer.Debt = 0;
            customer.Name = customer.Name.ToLower();

            //assign the customer a laundryId and/or employeeId 
            if (userRole == RoleNames.LaundryEmployee && employee != null)
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
                _context.Laundries.Find(laundry.Id).NoOfCustomers += 1;
            }
           
            await _context.Customers.AddAsync(customer);
            await _context.SaveChangesAsync();
            return customer;
        }
    
        public IEnumerable<CustomerDto> SearchForCustomer(string searchParam,string paramType)
        {
            List<Customer> customer;
            if (paramType == "email")
                customer = _context.Customers.Where(x => x.Username.Contains(searchParam)).ToList();
            else if (paramType == "name")
                customer = _context.Customers.Where(x => x.Name.Contains( searchParam)).ToList();
            else
                customer = _context.Customers.Where(x => x.PhoneNumber.Contains(searchParam)).ToList();

            IEnumerable<CustomerDto> customers=mapper.Map<IEnumerable<CustomerDto>>(customer);
            return customers;
                
        }
    }

}
