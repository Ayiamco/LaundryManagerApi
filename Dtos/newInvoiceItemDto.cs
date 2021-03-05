using LaundryApi.Entites;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace LaundryApi.Dtos
{
    public class NewInvoiceItemDto
    {
        public Guid ServiceId { get; set; }

        [Required]
        public int Quantity { get; set; }

          
    }
}
