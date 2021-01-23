
using LaundryApi.Dtos;
using LaundryApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LaundryApi.Interfaces
{
    public interface IManagerRepository
    {
        public ApplicationUser GetUserByUsername(string username);
        public LoginResponseDto GetLoginResponse(string username,string password);
        public Task<LaundryDto> CreateLaundryAsync(NewLaundryDto newLaundryDto);
        public Task<EmployeeDto> CreateEmployeeAsync(NewEmployeeDto newEmployeeDto);
        public Task<bool> SendPasswordReset(string username);

        public bool IsPasswordResetLinkValid(string username, string resetLinkId);
        public void ResetPassword( ForgotPasswordDto dto,string resetLinkId);
    }
}
