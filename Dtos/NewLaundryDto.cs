using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace LaundryApi.Dtos
{
    public class NewLaundryDto
    {
        [Required(ErrorMessage ="Username is required")]
        [EmailAddress(ErrorMessage ="username must be a valid email address") ]
        public string  Username {get ; set;}

        
        [Required(ErrorMessage = "Username is required")]
        [StringLength(11 ,ErrorMessage="password must be at least 11 characters")]
        public string Password { get; set; }

        [Required]
        [Compare("Password")]
        public string ConfirmPassword { get; set; }

        [Required]
        public string LaundryName { get; set; }

        [Required]
        public string FullName { get; set; }

        [Required]
        [StringLength(11,ErrorMessage ="Exceeds max number of characters")]
        public string PhoneNumber { get; set; }

        [Required]
        public string Address { set; get; }
    }
}
