using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace LaundryApi.Models
{
    public class Laundry
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid LaundryId { get; set; }

        //Username is an alternate key
        [EmailAddress]
        public string Username { get; set; }

        [Required]
        public string LaundryName { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        [StringLength(11)]
        public string PhoneNumber { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        [Required]
        public string Address { get; set;}

        [Required]
        public Int64 NoOfCustomers { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,4)")]
        public decimal TotalRevenue { get; set; }

    }
}
