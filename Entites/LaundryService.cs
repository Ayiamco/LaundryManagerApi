﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace LaundryApi.Entites
{
    public class LaundryService
    {
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,4)")]
        public decimal Price { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        [Required]
        public DateTime UpdatedAt { get; set; }

        public Laundry Laundry { get; set; }

        [Required]
        public Guid LaundryId { get; set; }

        [Required]
        public bool IsDeleted { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,4)")]
        public decimal Revenue { get; set; }
    }
}

