﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace LaundryApi.Dtos
{
    public class LaundryDto
    {
        
        public Guid LaundryId { get; set; }

       
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
        public string Address { get; set; }

        public Int64 NoOfCustomers { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,4)")]
        public decimal TotalRevenue { get; set; }
    }
}
