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

        public InvoiceDto Invoice { get; set; }

        public Guid InvoiceId { get; set; }

        [Required]
        public Guid ServiceId { get; set; }

        public ServiceDto Service { get; set; }

        [Required]
        public int Quantity { get; set; }

        public bool IsCollected { get; set; }
    }
}
