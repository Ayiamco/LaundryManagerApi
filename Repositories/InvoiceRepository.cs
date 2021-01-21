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

namespace LaundryApi.Repositories
{
    public class InvoiceRepository : IInvoiceRepository
    {
        private readonly LaundryApiContext _context;
        private readonly IMapper mapper;
        private readonly ICustomerRepository customerRepository;

        public InvoiceRepository(LaundryApiContext _context, IMapper mapper,ICustomerRepository customerRepository)

        {
            this._context = _context;
            this.mapper = mapper;
            this.customerRepository = customerRepository;
        } 

        public async Task<InvoiceDto> ReadInvoice(Guid invoiceId)
        {

            var invoice = await _context.Invoices.FindAsync(invoiceId);
            var invoiceDto = mapper.Map<InvoiceDto>(invoice);
            return invoiceDto;
        }

        public InvoiceDto AddInvoice(NewInvoiceDto newInvoiceDto,string laundryUsername)
        {
            if (customerRepository.GetCustomer(newInvoiceDto.CustomerId) == null)
                throw new Exception(ErrorMessage.EntityDoesNotExist);
            //get invoice total 
            decimal invoiceTotal=newInvoiceDto.InvoiceItems.GetInvoiceTotal();

            Invoice invoice = new Invoice()
            {
                Amount = invoiceTotal,
                CustomerId=newInvoiceDto.CustomerId,
                CreatedAt=DateTime.Now,
            };

            //add invoice to db
             _context.Invoices.Add(invoice);

            //update customer  total purchase
            var customerInDb=_context.Customers.SingleOrDefault(x => x.CustomerId == newInvoiceDto.CustomerId);
            customerInDb.TotalPurchase += invoiceTotal;

            //update laundry total revenue
            var laundryInDb = _context.Laundries.SingleOrDefault(x => x.Username == laundryUsername);
            laundryInDb.TotalRevenue = laundryInDb.TotalRevenue + invoiceTotal;

            //add invoice items to db
            List<InvoiceItem> invoiceItems=new List<InvoiceItem>();
            foreach(NewInvoiceItemDto item in newInvoiceDto.InvoiceItems)
            {
                invoiceItems.Add(new InvoiceItem() {
                    ServiceId=item.ServiceId,
                    Quantity=item.Quantity,
                    InvoiceId=invoice.InvoiceId
                });
            }
            _context.AddRange(invoiceItems);

            //persist changes
            _context.SaveChanges();

            //map entity to Dto
            InvoiceDto invoiceDto = mapper.Map<InvoiceDto>(invoice);

            return invoiceDto;
            
        }

        public IEnumerable<InvoiceDto> GetInvoices()
        {
            var invoicesList=_context.Invoices.ToList();
            List<InvoiceDto> obj = new List<InvoiceDto>();
            foreach(Invoice invoice in invoicesList)
            {
                var objDto = mapper.Map<InvoiceDto>(invoice);
                obj.Add(objDto);
            }
            return obj;
        }

        public IEnumerable<InvoiceDto> GetInvoices(int batchNumber, int batchQuantity)
        {
           var  invoicesList=_context.Invoices.ToList().Skip((batchNumber - 1) * batchQuantity).Take(batchQuantity);
            List<InvoiceDto> obj = new List<InvoiceDto>();
            foreach (Invoice invoice in invoicesList)
            {
                var objDto = mapper.Map<InvoiceDto>(invoice);
                obj.Add(objDto);
            }
            return obj;
        }

        
    }
}