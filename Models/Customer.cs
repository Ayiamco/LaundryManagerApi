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

        [Required]
        [EmailAddress]
        public string Username { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Address { get; set; }

        [Required]
        public string PhoneNumber { get; set; }

        public Guid EmployeeId { get; set; }
        public Guid LaundryId { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        [Required]
        public DateTime UpdatedAt { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,4)")]
        public decimal TotalPurchase { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal Debt { get; set; }
        public bool IsDeleted { get; set; }

        
    }
}
