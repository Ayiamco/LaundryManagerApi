using LaundryApi.Entites;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace LaundryApi.Dtos
{
    public class InvoiceDto
    {
        public Guid Id { set; get; }

        [Required]
        [Column(TypeName = "decimal(18,4)")]
        public decimal Amount { get; set; }

        public CustomerDto Customer { get; set; }

        [Required]
        public Guid CustomerId { get; set; }

        public IEnumerable<InvoiceItemDtoLight> InvoiceItems { get; set; }

        [Required]
        public bool IsPaidFor { set; get; }

        [Required]
        public bool IsCollected { set; get; }

        public decimal AmountPaid { get; set; }

        public string Remark { get; set; }

        
    }
}
