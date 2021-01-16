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
    public class InvoiceDbServcie:IInvoiceDbService
    {
        private readonly LaundryApiContext _context;
        private readonly IMapper mapper;

        public InvoiceDbServcie(LaundryApiContext _context, IMapper mapper)
        {
            this._context = _context;
            this.mapper = mapper;
        }

        public async Task<InvoiceDto> ReadInvoice(Guid invoiceId)
        {
            
            var invoice =await _context.Invoices.FindAsync(invoiceId);
            var invoiceDto=mapper.Map<InvoiceDto>(invoice);
            return invoiceDto;
        }

        public async Task<InvoiceDto> AddInvoice(InvoiceDto invoiceDto)
        {
          
            var invoice=mapper.Map<Invoice>(invoiceDto);
            invoice.Date = DateTime.Now;
            await _context.Invoices.AddAsync(invoice);
            await _context.SaveChangesAsync();

            invoiceDto = mapper.Map<InvoiceDto>(invoice);
            return invoiceDto;
        }
    }
}
