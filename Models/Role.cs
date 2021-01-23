using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LaundryApi.Models
{
    //creating user roles
    //List<Role> applicationRoles = new List<Role>();
    //applicationRoles.Add(new Role() { Name = RoleNames.LaundryOwner });
    //applicationRoles.Add( new Role() { Name = RoleNames.LaundryEmployee });
    //applicationRoles.Add( new Role() { Name = RoleNames.Admin });
    //_context.Roles.AddRange(applicationRoles);


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
