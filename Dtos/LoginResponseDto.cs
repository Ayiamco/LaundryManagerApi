using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LaundryApi.Dtos
{
    public class LoginResponseDto
    {
        public LaundryDto Laundry { get; set; }
        public string PasswordHash { get; set; }
        public string UserRole { get; set; }
    }
}
