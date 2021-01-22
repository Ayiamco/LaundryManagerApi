using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LaundryApi.Models
{
    public static class RoleNames
    {
        public static string LaundryOwner = "LaundryOwner";
        public static string LaundryEmployee = "LaundryEmployee";
        public static string Admin = "Admin";
    }
       
    public class Role
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        public string Name { get; set; }
    }
}
