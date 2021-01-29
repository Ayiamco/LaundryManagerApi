using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LaundryApi.Dtos
{
    public class EmployeeDtoPartial
    {
        
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Password { get; set; }

        public string Username { get; set; }

        public string PhoneNumber { get; set; }

        
        public string Address { get; set; }

      
        public decimal Revenue { get; set; }

   
        public int NoOfCustomers { get; set; }


    }
}

