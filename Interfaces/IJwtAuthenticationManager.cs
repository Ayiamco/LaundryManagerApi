using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LaundryApi.Dtos;

namespace LaundryApi.Interfaces
{
    public interface IJwtAuthenticationManager
    {
        public string GetToken(UserLoginDto _user);
    }
}
