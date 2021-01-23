using LaundryApi.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace LaundryApi.Dtos
{
    public class EmployeeDto
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }


        [EmailAddress]
        public string Username { get; set; }

        public ApplicationUser Laundry { get; set; }

        
        [ForeignKey("Laundry")]
        public string LaundryId { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        [StringLength(11)]
        public string PhoneNumber { get; set; }

        [Required]
        public string Address { get; set; }

        public int NoOfCustomers { get; set; }
        public decimal Revenue { get; set; }
    }
}
