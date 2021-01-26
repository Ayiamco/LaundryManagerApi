//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;
//using LaundryApi.Dtos;
//using LaundryApi.Interfaces;
//using LaundryApi.Infrastructure;
//using AutoMapper;
//using LaundryApi.Models;
//using static LaundryApi.Infrastructure.LaundryApiExtenstionMethods;
//using static LaundryApi.Infrastructure.HelperMethods;
//using Microsoft.EntityFrameworkCore;

//namespace LaundryApi.Repositories
//{
//    public class InvoiceRepository : IInvoiceRepository
//    {
//        private readonly LaundryApiContext _context;
//        private readonly IMapper mapper;
//        private readonly ICustomerRepository customerRepository;

//        public InvoiceRepository(LaundryApiContext _context, IMapper mapper, ICustomerRepository customerRepository)

//        {
//            this._context = _context;
//            this.mapper = mapper;
//            this.customerRepository = customerRepository;
//        }

//        public async Task<InvoiceDto> ReadInvoice(Guid invoiceId)
//        {

//            var invoice = await _context.Invoices.Include("Customer").FirstOrDefaultAsync(x=> x.Id==invoiceId);
//            var invoiceDto = mapper.Map<InvoiceDto>(invoice);
//            return invoiceDto;
//        }

//        public InvoiceDto AddInvoice(NewInvoiceDto newInvoiceDto)
//        {
//            try
//            {
//                if (customerRepository.GetCustomer(newInvoiceDto.CustomerId) == null)
//                    throw new Exception(ErrorMessage.EntityDoesNotExist);

//                //get invoice total 
//                decimal invoiceTotal = newInvoiceDto.InvoiceItems.GetInvoiceTotal();

//                //create invoice object
//                Invoice invoice = new Invoice()
//                {
//                    Amount = invoiceTotal,
//                    CustomerId = newInvoiceDto.CustomerId,
//                    CreatedAt = DateTime.Now,
//                };

//                //add invoice to db context
//                _context.Invoices.Add(invoice);

//                //update customer  total purchase
//                var customerInDb = _context.Customers.SingleOrDefault(x => x.Id == newInvoiceDto.CustomerId);
//                customerInDb.TotalPurchase += invoiceTotal;

//                //update laundry total revenue
//                var laundryInDb = _context.ApplicationUsers.SingleOrDefault(x => x.Id == customerInDb.ApplicationUserId);
//                laundryInDb.Revenue += invoiceTotal;

//                //add invoice items to db context
//                List<InvoiceItem> invoiceItems = new List<InvoiceItem>();
//                foreach (NewInvoiceItemDto item in newInvoiceDto.InvoiceItems)
//                {
//                    invoiceItems.Add(new InvoiceItem()
//                    {
//                        ServiceId = item.ServiceId,
//                        Quantity = item.Quantity,
//                        InvoiceId = invoice.Id
//                    });
//                }
//                _context.AddRange(invoiceItems);

//                //coomplete transaction
//                _context.SaveChanges();

//                //map entity to Dto
//                InvoiceDto invoiceDto = mapper.Map<InvoiceDto>(invoice);

//                return invoiceDto;
//            }
//            catch(Exception e)
//            {
//                if (e.Message == ErrorMessage.EntityDoesNotExist)
//                    throw new Exception(ErrorMessage.EntityDoesNotExist);

//                throw new Exception(ErrorMessage.FailedDbOperation);
//            }
            

//        }

//        public IEnumerable<InvoiceDto> GetInvoices()
//        {
//            var invoicesList = _context.Invoices.ToList();
//            List<InvoiceDto> obj = new List<InvoiceDto>();
//            foreach (Invoice invoice in invoicesList)
//            {
//                var objDto = mapper.Map<InvoiceDto>(invoice);
//                obj.Add(objDto);
//            }
//            return obj;
//        }

//        public IEnumerable<InvoiceDto> GetInvoices(int batchNumber, int batchQuantity)
//        {
//            var invoicesList = _context.Invoices.ToList().Skip((batchNumber - 1) * batchQuantity).Take(batchQuantity);
//            List<InvoiceDto> obj = new List<InvoiceDto>();
//            foreach (Invoice invoice in invoicesList)
//            {
//                var objDto = mapper.Map<InvoiceDto>(invoice);
//                obj.Add(objDto);
//            }
//            return obj;
//        }

//        public async Task<InvoiceDto> ReadCompleteInvoiceAsync(Guid invoiceId)
//        {
//            try
//            {
//                //read all the invoice items that match the invoiceId
//                var invoiceItems = mapper.Map<IEnumerable<InvoiceItemDto>>(_context.InvoiceItems.Where(x => x.InvoiceId == invoiceId).ToList());

//                //get the invoice that matches the invoice Id 
//                var invoice = await _context.Invoices.FindAsync(invoiceId);

//                //map concrete Invoice object to Dto
//                InvoiceDto invoiceDto = mapper.Map<InvoiceDto>(invoice);
//                invoiceDto.InvoiceItems = invoiceItems;

//                return invoiceDto;
//            }
//            catch(Exception e)
//            {
//                //if exception is thrown because id does not exist
//                //throw new Exception(ErrorMessage.EntityDoesNotExist)

//                throw new Exception(ErrorMessage.FailedDbOperation);
//            }
            


//        }
//    }
//}