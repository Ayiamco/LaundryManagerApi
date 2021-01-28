using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LaundryApi.Dtos;
using LaundryApi.Interfaces;
using LaundryApi.Infrastructure;
using AutoMapper;
using LaundryApi.Models;
using static LaundryApi.Infrastructure.LaundryApiExtenstionMethods;
using static LaundryApi.Infrastructure.HelperMethods;
using Microsoft.EntityFrameworkCore;

namespace LaundryApi.Repositories
{
    public class InvoiceRepository : IInvoiceRepository
    {
        private readonly LaundryApiContext _context;
        private readonly IMapper mapper;
        private readonly ICustomerRepository customerRepository;
        private readonly IRepositoryHelper repositoryHelper;

        public InvoiceRepository(LaundryApiContext _context, IMapper mapper, ICustomerRepository customerRepository,IRepositoryHelper repositoryHelper)

        {
            this._context = _context;
            this.mapper = mapper;
            this.customerRepository = customerRepository;
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
                
           
                //get the invoice total
                decimal invoiceTotal = newInvoiceDto.InvoiceItems.GetInvoiceTotal();

                //update customer and laundry
                customerInDb.Debt += invoiceTotal - newInvoiceDto.AmountPaid;
                if (newInvoiceDto.AmountPaid > 0)
                {
                    Laundry laundry = _context.Laundries.Find(customerInDb.LaundryId);
                    laundry.Revenue += newInvoiceDto.AmountPaid;
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

        public IEnumerable<InvoiceDto> GetInvoices()
        {
            var invoicesList = _context.Invoices;
            IEnumerable<InvoiceDto> obj = (IEnumerable<InvoiceDto>)mapper.Map<InvoiceDto>(invoicesList);
            
            return obj;
        }

        public IEnumerable<InvoiceDto> GetInvoices(int pageNumber, int pageSize)
        {
            var invoicesList = _context.Invoices.ToList().Skip((pageNumber - 1) * pageSize).Take(pageSize);
            List<InvoiceDto> obj = new List<InvoiceDto>();
            foreach (Invoice invoice in invoicesList)
            {
                var objDto = mapper.Map<InvoiceDto>(invoice);
                obj.Add(objDto);
            }
            return obj;
        }

        public async Task<InvoiceDto> ReadCompleteInvoiceAsync(Guid invoiceId)
        {
            try
            {
                //read all the invoice items that match the invoiceId
                var invoiceItems = mapper.Map<IEnumerable<InvoiceItemDto>>(_context.InvoiceItems.Where(x => x.InvoiceId == invoiceId).ToList());

                //get the invoice that matches the invoice Id 
                var invoice = await _context.Invoices.FindAsync(invoiceId);

                //map concrete Invoice object to Dto
                InvoiceDto invoiceDto = mapper.Map<InvoiceDto>(invoice);
                invoiceDto.InvoiceItems = invoiceItems;

                return invoiceDto;
            }
            catch (Exception e)
            {
                //if exception is thrown because id does not exist
                //throw new Exception(ErrorMessage.EntityDoesNotExist)

                throw new Exception(ErrorMessage.FailedDbOperation);
            }



        }
        
        public void PayForInvoice (InvoiceDto invoice)
        {
            //get the invoice and update it
            var invoiceInDb=_context.Invoices.SingleOrDefault( x=> x.Id== invoice.Id);
            invoiceInDb.AmountPaid += invoice.AmountPaid;
            invoiceInDb.IsPaidFor = invoiceInDb.AmountPaid > invoiceInDb.Amount;

            //save changes
            _context.SaveChanges();
            return;
        }
    }
}