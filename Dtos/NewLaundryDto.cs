using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LaundryApi.Dtos
{
    public class NewLaundryDto
    {
        public string  Username {get ; set;}
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public string LaundryName { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { set; get; }
    }
}
