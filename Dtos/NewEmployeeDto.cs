using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace LaundryApi.Dtos
{
    public class NewEmployeeDto
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }


        [EmailAddress]
        public string Username { get; set; }


        [Required]
        [ForeignKey("Laundry")]
        public Guid LaundryId { get; set; }

        [Required]
        public string Password { get; set; }

        [Compare("Password")]
        public string ConfirmPassword { get; set; }

        [Required]
        [StringLength(11)]
        public string PhoneNumber { get; set; }

        [Required]
        public string Address { get; set; }

        public int NoOfCustomers { get; set; }
        public decimal Revenue { get; set; }
    }
}
