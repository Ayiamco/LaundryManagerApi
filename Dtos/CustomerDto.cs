using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace LaundryApi.Dtos
{
    public class CustomerDto
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [EmailAddress]
        public string Username { get; set; }

        public string Name { get; set; }

        public string Address { get; set; }

        public string PhoneNumber { get; set; }
        public decimal TotalPurchase { get; set; }
        public Guid EmployeeId { get; set; }
        public Guid LaundryId { get; set; }

        public decimal Debt { get; set; }
        public bool IsDeleted { get; set; }

        public DateTime CreatedAt { get; set; }

    }
}
