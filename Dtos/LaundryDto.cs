using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace LaundryApi.Dtos
{
    public class LaundryDto
    {
        public Guid Id { get; set; }
   
        [Phone]
        public string PhoneNumber { get; set; }

        public string Address { get; set; }

        public string LaundryName { get; set; }

        public int NoOfCustomers { get; set; }
        public int NoOfEmployees { get; set; }
        public decimal Revenue { get; set; }
    }

    public class NewLaundryDto:LaundryDto
    {
        public string Password { get; set; }

        [Compare("Password")]
        public string ConfirmPassword { get; set; }
    }
}
