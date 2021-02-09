using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace LaundryApi.Entites
{
    public class EmployeeInTransit
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public Guid LaundryId { get; set; }

        public Laundry Laundry { get; set; }
    }
}
