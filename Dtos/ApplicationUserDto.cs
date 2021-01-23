using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace LaundryApi.Dtos
{
    public class ApplicationUserDto
    {
        public Guid Id { get; set; }
        public string LaundryName { get; set; }

        [Required]
        public string FullName { get; set; }

        //Username is an alternate key
        [EmailAddress]
        public string Username { get; set; }

        [Required]
        [StringLength(11)]
        public string PhoneNumber { get; set; }

        [Required]
        public string Address { get; set; }

        
        [Column(TypeName = "decimal(18,4)")]
        public decimal Revenue { get; set; }


        public int? NoOfCustomers { get; set; }

        public int? NoOfEmployees { get; set; }
    }
}
