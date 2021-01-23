using LaundryApi.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LaundryApi.Interfaces
{
    public interface IManagerRepository
    {
        public LoginResponseDto GetUserByUsername(string username);
        public Task<LaundryDto> CreateLaundryAsync(NewLaundryDto newLaundryDto);
        public bool SendPasswordReset(string username);

        public bool IsPasswordResetLinkValid(string username, string resetLinkId);
        public void ResetPassword( ForgotPasswordDto dto,string resetLinkId);
    }
}
