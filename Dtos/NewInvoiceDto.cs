using LaundryApi.Entites;
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

        [Required]
        [Column(TypeName = "decimal(18,4)")]
        public decimal AmountPaid { get; set; }

        public string Remark { get; set; }

    }

    public class PaymentDto
    {
        public decimal Amount { get; set; }
        public Guid CustomerId { get; set; }
    }
}

