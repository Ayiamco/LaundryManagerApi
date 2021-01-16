using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LaundryApi.Dtos;
using LaundryApi.Models;

namespace LaundryApi.Interfaces
{
    public interface IInvoiceDbService
    {
        public Task<InvoiceDto> AddInvoice(InvoiceDto invoiceDto);

        public Task<InvoiceDto> ReadInvoice(Guid invoiceId);
    }
}
