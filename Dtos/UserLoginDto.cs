using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LaundryApi.Dtos
{
    public class UserLoginDto
    {
        public string Password { get; set; }
        public string Username { get; set; }
    }
}
