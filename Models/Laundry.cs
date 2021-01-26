using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace LaundryApi.Models
{
    public class Laundry:ApplicationUser
    {
        [Required]
        public int NoOfEmployees { get; set; }  

    }
}
