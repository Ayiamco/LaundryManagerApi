using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LaundryApi.Dtos;
using LaundryApi.Interfaces;
using LaundryApi.Infrastructure;
using AutoMapper;
using LaundryApi.Entites;
using static LaundryApi.Infrastructure.LaundryApiExtenstionMethods;
using static LaundryApi.Infrastructure.HelperMethods;
using Microsoft.EntityFrameworkCore;
using LaundryApi.Models;

namespace LaundryApi.Repositories
{
    public class InvoiceRepository : IInvoiceRepository
    {
        private readonly LaundryApiContext _context;
        private readonly IMapper mapper;
        private readonly IRepositoryHelper repositoryHelper;

        public InvoiceRepository(LaundryApiContext _context, IMapper mapper,IRepositoryHelper repositoryHelper)

        {
            this._context = _context;
            this.mapper = mapper;
            this.repositoryHelper = repositoryHelper;
        }

        public async Task<InvoiceDto> ReadInvoice(Guid invoiceId)
        {

            var invoice = await _context.Invoices.Include("Customer").FirstOrDefaultAsync(x => x.Id == invoiceId);
            var invoiceDto = mapper.Map<InvoiceDto>(invoice);
            return invoiceDto;
        }

        public InvoiceDto AddInvoice(NewInvoiceDto newInvoiceDto, string userRole,string username)
        {
            try
            {
                //check if customer exist 
                var customerInDb = _context.Customers.SingleOrDefault(x => x.Id == newInvoiceDto.CustomerId);
                if (customerInDb == null)
                    throw new Exception(ErrorMessage.EntityDoesNotExist);

                //check if the customer is owned by the current user
                if (userRole == RoleNames.LaundryOwner)
                {
                    if ( repositoryHelper.GetLaundryByUsername(username).Id != customerInDb.LaundryId)
                        throw new Exception(ErrorMessage.InvalidToken);
                }
                else
                {
                    if (repositoryHelper.GetEmployeeByUsername(username).LaundryId != customerInDb.LaundryId)
                        throw new Exception(ErrorMessage.InvalidToken);
                }

                //get the invoice total, update customer,laundry AND EMPLOYEE
                decimal invoiceTotal = newInvoiceDto.InvoiceItems.GetInvoiceTotal();
                customerInDb.Debt += invoiceTotal - newInvoiceDto.AmountPaid;
                if (newInvoiceDto.AmountPaid > 0)
                {
                    customerInDb.TotalPurchase += newInvoiceDto.AmountPaid;
                    Laundry laundry = _context.Laundries.Find(customerInDb.LaundryId);
                    laundry.Revenue += newInvoiceDto.AmountPaid;
                    if(customerInDb.EmployeeId != new Guid())
                    {
                        Employee employee = _context.Employees.Find(customerInDb.EmployeeId);
                        employee.Revenue += newInvoiceDto.AmountPaid;
                    }
                }

                //create the invoice object and add the invoice to the db context 
                Invoice invoice = new Invoice()
                {
                    Amount = invoiceTotal,
                    CustomerId = newInvoiceDto.CustomerId,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                    IsCollected = false,
                    IsPaidFor = newInvoiceDto.AmountPaid==invoiceTotal,
                    AmountPaid= newInvoiceDto.AmountPaid
                };
                _context.Invoices.Add(invoice);

                //create the list of invoice items and add them to the dbcontext
                List<InvoiceItem> invoiceItems = new List<InvoiceItem>();
                foreach (NewInvoiceItemDto item in newInvoiceDto.InvoiceItems)
                {
                    invoiceItems.Add(new InvoiceItem()
                    {
                        ServiceId = item.Service.Id,
                        Quantity = item.Quantity,
                        InvoiceId = invoice.Id,

                    });
                }
                _context.AddRange(invoiceItems);

                //coomplete transaction
                _context.SaveChanges();

                //create return obj
                InvoiceDto invoiceDto = mapper.Map<InvoiceDto>(invoice);
                invoiceDto.Customer = mapper.Map<CustomerDto>(customerInDb);
                return invoiceDto;
            }
            catch (Exception e)
            {
                if (e.Message == ErrorMessage.EntityDoesNotExist)
                    throw new Exception(ErrorMessage.EntityDoesNotExist);

                else if (e.Message == ErrorMessage.InvalidToken)
                    throw new Exception(ErrorMessage.InvalidToken);

                throw new Exception(ErrorMessage.FailedDbOperation);
            }


        }

        public PagedList<InvoiceDto> GetInvoices(int pageNumber, int pageSize)
        {
            var invoicesList = _context.Invoices.ToList();
            var page=invoicesList.Skip((pageNumber - 1) * pageSize).Take(pageSize);

            PagedList<InvoiceDto> obj = new PagedList<InvoiceDto>()
            {
                Data= mapper.Map<IEnumerable<InvoiceDto>>(page),
                PageIndex=pageNumber,
                PageSize=pageSize,
                MaxPageIndex=invoicesList.Count,
            };

            return obj;
        }
        public PagedList<InvoiceDto> GetInvoices(int pageNumber, int pageSize,string username,string userRole)
        {
            //get the laundry 
            Guid laundryId;
            if (userRole == RoleNames.LaundryEmployee)
                laundryId = _context.Employees.SingleOrDefault(x => x.Username == username).LaundryId;
            else
                laundryId = _context.Laundries.SingleOrDefault(x => x.Username == username).Id;

            //get the laundry customers and use the customers to get the invoices from the invoice table
            IEnumerable<Customer> customers = _context.Customers.Where(x => x.LaundryId == laundryId);
            List<Invoice> invoices= new List<Invoice>();
            foreach(Customer customer in customers)
            {
                invoices.AddRange(_context.Invoices.Where(x=> x.CustomerId==customer.Id));
            }
            
            //do the pagination and select 
            var page = invoices.OrderBy(x => x.Amount).Skip((pageNumber - 1) * pageSize).Take(pageSize);
            PagedList<InvoiceDto> obj = new PagedList<InvoiceDto>()
            {
                Data = mapper.Map<IEnumerable<InvoiceDto>>(page),
                PageIndex = pageNumber,
                PageSize = pageSize,
                MaxPageIndex = invoices.Count,
            };

            return obj;
        }

        public async Task<InvoiceDto> ReadCompleteInvoiceAsync(Guid invoiceId)
        {
            try
            {
                //read all the invoice items that match the invoiceId
                var invoiceItems = mapper.Map<IEnumerable<InvoiceItemDtoLight>>(_context.InvoiceItems.Include("Service").Where(x => x.InvoiceId == invoiceId).ToList());
                if (invoiceItems.Count() == 0)
                    throw new Exception(ErrorMessage.EntityDoesNotExist);
                //get the invoice that matches the invoice Id 
                var invoice = await _context.Invoices.FindAsync(invoiceId);

                //map concrete Invoice object to Dto
                InvoiceDto invoiceDto = mapper.Map<InvoiceDto>(invoice);
                invoiceDto.InvoiceItems = invoiceItems;

                return invoiceDto;
            }
            catch (Exception e)
            {
                if (e.Message == ErrorMessage.EntityDoesNotExist)
                    throw new Exception(ErrorMessage.EntityDoesNotExist);

                throw new Exception(ErrorMessage.FailedDbOperation);
            }



        }

        public void DepositCustomerPayment(Guid customerId, decimal amtDeposited)
        {
            IEnumerable<Invoice> invoices = _context.Invoices
                .Where(x => x.Id == customerId && !x.IsPaidFor).OrderBy(x => x.CreatedAt);
            if (invoices.Count() == 0)
                throw new Exception(ErrorMessage.NoEntityMatchesSearch);

            //loop thru the invoices to find unpaid invoices
            UpdateInvoicesWIthDeposit(invoices, amtDeposited);

            //update the laundry,employee and customer object
            var customer = _context.Customers.Find(customerId);
            customer.TotalPurchase += amtDeposited;
            if(customer.EmployeeId != new Guid())
            {
                var employee = _context.Employees.Find(customer.EmployeeId);
                employee.Revenue += amtDeposited;
            }
            Laundry laundry=_context.Laundries.Find(customer.LaundryId);
            laundry.Revenue += amtDeposited;

            _context.SaveChanges();
            
        }

        public IEnumerable<InvoiceDto> FetchCustomerInvoices(Guid customerId)
        {
           var invoices= _context.Invoices.Where(x => x.CustomerId == customerId).OrderBy(x => x.CreatedAt);
            var invoiceDtos = mapper.Map<IEnumerable<InvoiceDto>>(invoices);
            return invoiceDtos;
            
        }

        private static void UpdateInvoicesWIthDeposit(IEnumerable<Invoice> invoices,decimal amtDeposited)
        {
            foreach (Invoice invoice in invoices)
            {
                decimal invoiceBalance = invoice.Amount - invoice.AmountPaid;
                if (invoiceBalance >= amtDeposited)
                {
                    invoice.AmountPaid += amtDeposited;
                    amtDeposited = 0;
                }
                else if (invoiceBalance < amtDeposited)
                {
                    invoice.AmountPaid += invoiceBalance;
                    amtDeposited -= invoiceBalance;
                    invoice.IsPaidFor = true;
                }
                
                if (amtDeposited == 0)
                    break;

            }
        }

    }
}