
using LaundryApi.Dtos;
using LaundryApi.Entites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LaundryApi.Interfaces
{
    public interface IManagerRepository
    {
        public Employee GetEmployeeByUsername(string username);
        public Laundry GetLaundryByUsername(string username);
        public LoginResponseDto GetLoginResponse(string username,string password);
       
        public Task<bool> SendPasswordReset(string username);
        public bool IsPasswordResetLinkValid(string username, string resetLinkId);
        public void ResetPassword( ForgotPasswordDto dto,string resetLinkId);
        public LoginResponseDto GetLoginResponse(string username, string password, string role);
    }
}
