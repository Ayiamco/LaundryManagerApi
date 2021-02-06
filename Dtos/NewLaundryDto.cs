using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace LaundryApi.Dtos
{
    public class NewLaundryDto
    {
       
        [EmailAddress(ErrorMessage ="username must be a valid email address") ]
        public string  Username {get ; set;}

       
        [StringLength(11 ,ErrorMessage="password must be at least 11 characters")]
        public string Password { get; set; }

       
        [Compare("Password")]
        public string ConfirmPassword { get; set; }

       
        public string LaundryName { get; set; }

       
        public string Name { get; set; }

      
        [StringLength(11,ErrorMessage ="Exceeds max number of characters")]
        public string PhoneNumber { get; set; }

        
        public string Address { set; get; }
    }
}
