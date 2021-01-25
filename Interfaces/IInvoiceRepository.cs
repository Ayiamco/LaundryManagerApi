using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LaundryApi.Dtos;
using LaundryApi.Models;

namespace LaundryApi.Interfaces
{
    public interface IInvoiceRepository
    {
        public InvoiceDto AddInvoice(NewInvoiceDto newInvoice);

        public Task<InvoiceDto> ReadInvoice(Guid invoiceId);

        public IEnumerable<InvoiceDto> GetInvoices(int batchQuantity, int batchNumber);
        public IEnumerable<InvoiceDto> GetInvoices();
    }
}
