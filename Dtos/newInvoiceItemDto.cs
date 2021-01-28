using LaundryApi.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace LaundryApi.Dtos
{
    public class NewInvoiceItemDto
    {
        public ServiceDto Service { get; set; }

        [Required]
        public int Quantity { get; set; }

        [Required]
        public string Description { set; get; }     
    }
}
