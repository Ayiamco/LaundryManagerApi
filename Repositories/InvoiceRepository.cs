using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LaundryApi.Dtos;
using LaundryApi.Interfaces;
using LaundryApi.Services;
using AutoMapper;
using LaundryApi.Models;

namespace LaundryApi.Services
{
    public class InvoiceDbServcie : IInvoiceRepository
    {
        private readonly LaundryApiContext _context;
        private readonly IMapper mapper;
        private readonly ICustomerRepository customerDbService;

        public InvoiceDbServcie(LaundryApiContext _context, IMapper mapper,ICustomerRepository customerDbService)
        {
            this._context = _context;
            this.mapper = mapper;
            this.customerDbService = customerDbService;
        }

        public async Task<InvoiceDto> ReadInvoice(Guid invoiceId)
        {

            var invoice = await _context.Invoices.FindAsync(invoiceId);
            var invoiceDto = mapper.Map<InvoiceDto>(invoice);
            return invoiceDto;
        }

        public async Task<InvoiceDto> AddInvoice(InvoiceDto invoiceDto)
        {

            var invoice = mapper.Map<Invoice>(invoiceDto);
            invoice.Date = DateTime.Now;
            await _context.Invoices.AddAsync(invoice);
            await _context.SaveChangesAsync();

            //update customer object
            customerDbService.UpdateTotalPurchase(invoiceDto.CustomerId, invoiceDto.Amount);

            invoiceDto = mapper.Map<InvoiceDto>(invoice);
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