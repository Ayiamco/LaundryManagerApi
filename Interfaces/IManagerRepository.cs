﻿
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
        public LoginResponseDto GetLoginResponse(string username,string password);
       
        public bool IsPasswordResetLinkValid( string resetLinkId);
        public void ResetPassword( ForgotPasswordDto dto,string resetLinkId);
        public LoginResponseDto GetLoginResponse(string username, string password, string role);
        public Task<bool> SendPasswordReset(ForgotPasswordDto dto);
    }
}
