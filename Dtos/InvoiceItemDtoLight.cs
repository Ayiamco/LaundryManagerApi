using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace LaundryApi.Dtos
{
    public class InvoiceItemDtoLight
    {
        public int Id { get; set; }

        public ServiceDto Service { get; set; }

        [Required]
        public int Quantity { get; set; }

        
    }
}
