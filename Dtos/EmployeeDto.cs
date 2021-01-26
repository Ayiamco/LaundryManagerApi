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

        [Required]
        public string Name { get; set; }

        public string Password { get; set; }

        [Required]
        [EmailAddress]
        public string Username { get; set; }

        [Required]
        [StringLength(11)]
        public string PhoneNumber { get; set; }

        [Required]
        public string Address { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,4)")]
        public decimal Revenue { get; set; }

        [Required]
        public int NoOfCustomers { get; set; }

        public LaundryDto Laundry { get; set; }

        [Required]
        public Guid LaundryId { get; set; }

    }
}
