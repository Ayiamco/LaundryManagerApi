using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LaundryApi.Dtos;
using LaundryApi.Models;

namespace LaundryApi.Interfaces
{
    public interface IInvoiceItemDbService
    {
        public void AddInvoiceItem(List<InvoiceItemDto> invoiceItemDto);
    }
}
