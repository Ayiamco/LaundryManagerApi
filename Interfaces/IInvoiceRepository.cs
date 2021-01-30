using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LaundryApi.Dtos;
using LaundryApi.Entites;
using LaundryApi.Models;

namespace LaundryApi.Interfaces
{
    public interface IInvoiceRepository
    {
        public InvoiceDto AddInvoice(NewInvoiceDto newInvoice,string userRole, string applicationUserUsername);

        public Task<InvoiceDto> ReadInvoice(Guid invoiceId);

        /// <summary>
        /// gets the paginated Invoices of all entries in the db
        /// </summary>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public PagedList<InvoiceDto> GetInvoices(int pageNumber, int pageSize);

        public Task<InvoiceDto> ReadCompleteInvoiceAsync(Guid invoiceId);

        /// <summary>
        /// gets the paginated invoice of aparticular laundry ordered by the date in descending order
        /// </summary>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <param name="username"></param>
        /// <param name="userRole"></param>
        /// <returns></returns>
        public PagedList<InvoiceDto> GetInvoices(int pageNumber, int pageSize, string username, string userRole);

        public void DepositCustomerPayment(Guid customerId, decimal amtDeposited);

        public IEnumerable<InvoiceDto> FetchCustomerInvoices(Guid customerId);
    }
}
