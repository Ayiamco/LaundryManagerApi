using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LaundryApi.Dtos;
using LaundryApi.Entites;

namespace LaundryApi.Interfaces
{
    public interface IInvoiceRepository
    {
        public InvoiceDto AddInvoice(NewInvoiceDto newInvoice,string userRole, string applicationUserUsername);

        public Task<InvoiceDto> ReadInvoice(Guid invoiceId);

        public IEnumerable<InvoiceDto> GetInvoices(int pageNumber, int pageSize);
        public IEnumerable<InvoiceDto> GetInvoices();

        public Task<InvoiceDto> ReadCompleteInvoiceAsync(Guid invoiceId);

        public void PayForInvoice(InvoiceDto invoice);

        public IEnumerable<InvoiceDto> GetInvoices(int pageNumber, int pageSize, string username, string userRole);
    }
}
