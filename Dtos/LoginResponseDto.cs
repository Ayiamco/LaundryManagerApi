using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LaundryApi.Models;

namespace LaundryApi.Dtos
{
    public class LoginResponseDto
    {
        public ApplicationUser User { get; set; }
        public string UserRole { get; set; }
    }
}
