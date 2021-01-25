using LaundryApi.Dtos;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace LaundryApi.Models
{
    public class Customer
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        //Email is an alternate key
        [Required]
        [EmailAddress]
        public string Username { get; set; }

        [Required]
        public string FullName { get; set; }

        [Required]
        public string Address { get; set; }

        public ApplicationUser ApplicationUser { get; set; }

        [Required]
        public Guid ApplicationUserId { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,4)")]
        public decimal TotalPurchase { get; set; }
    }
}
