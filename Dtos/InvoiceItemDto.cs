using LaundryApi.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace LaundryApi.Dtos
{
    public class InvoiceItemDto
    {
        public int Id { get; set; }

        public Invoice Invoice { get; set; }

        [Required]
        [ForeignKey("Invoice")]
        public Guid InvoiceId { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,4)")]
        public decimal Price { get; set; }
    }
}
