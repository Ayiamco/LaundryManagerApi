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

        [Required]
        public DateTime CreatedAt { get; set; }

        public Customer Customer { get; set; }

        [Required]
        public Guid CustomerId { get; set; }

        public Guid LaundryId { get; set; }

        public Laundry Laundry { get; set; }


        [ForeignKey("InvoiceId")]
        public ICollection<InvoiceItem> InvoiceItems { get; set; }

        public string Remark { get; set; }
        public DateTime UpdatedAt { get; set; }

        public bool IsCollected { get; set; }

        public bool IsPaidFor { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,4)")]
        public decimal AmountPaid { get; set; }


    }
}
