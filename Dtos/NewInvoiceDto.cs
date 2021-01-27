using LaundryApi.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace LaundryApi.Dtos
{
    public class NewInvoiceDto
    {
        public IEnumerable<NewInvoiceItemDto> InvoiceItems { get; set; }

        public Guid CustomerId { get; set; }

        //new invoices are always not paid for and not collected
        public bool IsPaidFor { get; set; }

        public bool IsCollected { get; set; }

    }
}

