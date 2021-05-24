using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using LaundryApi.Models;

namespace LaundryApi.Entites
{
    public class Laundry
    {
        public Guid Id { get; set; }
        public int NoOfEmployees { get; set; }  
        public string Name { get; set; }
        public ICollection<UserProfle> Employees { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public Location Address { get; set; }

    }
}
