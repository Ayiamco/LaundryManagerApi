using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using LaundryApi.Models;

namespace LaundryApi.Entites
{
    public class UserProfle 
    {
        public int Id { get; set; }
        public Laundry Laundry { get; set; }
        public Guid LaundryId { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
