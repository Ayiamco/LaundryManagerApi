using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LaundryApi.Models;
using LaundryApi.Dtos;

namespace LaundryApi.Infrastructure
{
    public static class LaundryApiExtenstionMethods
    {
        public static decimal GetInvoiceTotal(this IEnumerable<NewInvoiceItemDto> invoiceItems)
        {
            decimal total = 0;
            foreach(NewInvoiceItemDto item in invoiceItems)
            {
                total += item.Price * item.Quantity;
            }
            return total;
        }
    }
}
