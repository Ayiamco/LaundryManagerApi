using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LaundryApi.Dtos
{
    public class LoginResponseDto
    {
        public ApplicationUserDto User { get; set; }
        public string UserRole { get; set; }
    }
}
