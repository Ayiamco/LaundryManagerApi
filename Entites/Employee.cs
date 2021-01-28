using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using LaundryApi.Models;

namespace LaundryApi.Entites
{
    public class Employee : ApplicationUser
    {
       
        public Laundry Laundry { get; set; }

        [Required]
        public Guid LaundryId { get; set; }
    }
}
