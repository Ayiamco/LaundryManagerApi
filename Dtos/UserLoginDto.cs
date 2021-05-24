using LaundryApi.Entites;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace LaundryApi.Dtos
{
    public class UserLoginDto
    {
        [Required]
        public string Password { get; set; }

        [Required]
        public string Username { get; set; }
    }

    public class UserDto
    {
        public int Id { get; set; }
        public Laundry Laundry { get; set; }
        public Guid LaundryId { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string Username { get; set; }
    }

    public class NewUserDto:UserLoginDto
    {
        [Compare("Password")]
        public string ConfirmPassword { get; set; }
    }
}
