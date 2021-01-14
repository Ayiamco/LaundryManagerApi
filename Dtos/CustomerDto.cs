using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LaundryApi.Dtos
{
    public class CustomerDto
    {
        public Guid CustomerId { get; set; }
        public string Email { get; set; }

        public string Name { get; set; }

        public string Address { get; set; }
        public decimal TotalPurchase { get; set; }
    }
}
