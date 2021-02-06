using AutoMapper;
using LaundryApi.Dtos;
using LaundryApi.Infrastructure;
using LaundryApi.Interfaces;
using LaundryApi.Entites;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using static LaundryApi.Infrastructure.HelperMethods;
using LaundryApi.Models;
using Microsoft.Extensions.Configuration;

namespace LaundryApi.Repositories
{
    public class ManagerRepository: IManagerRepository
    {
        private readonly LaundryApiContext _context;
        private readonly IMapper mapper;
        private readonly IRepositoryHelper _contextHelper;
        private readonly IMailService mailService;
        private readonly IConfiguration config;
        public ManagerRepository(LaundryApiContext _context, IMapper mapper,
            IRepositoryHelper _contextHelper,IMailService mailService,IConfiguration config)
        {
            this.config = config;
            this.mapper = mapper;
            this._context = _context;
            this._contextHelper = _contextHelper;
            this.mailService = mailService;
        }
        
        public Employee GetEmployeeByUsername(string username)
        {
            try
            {
                var employeeInDb = _context.Employees.AsQueryable().SingleOrDefault(_user => _user.Username == username);
                if (employeeInDb == null)
                    throw new Exception(ErrorMessage.UserDoesNotExist);

                return employeeInDb;

            }
            catch(Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public bool IsPasswordResetLinkValid(string resetLinkId)
        {
            var usernameHash = resetLinkId[11..];
            var laundryOwner = _context.Laundries.SingleOrDefault(x => x.UsernameHash == usernameHash); 
            var employee=  _context.Employees.SingleOrDefault(x => x.UsernameHash == usernameHash);

            var user = laundryOwner ?? (ApplicationUser) employee;

            return user.UsernameHash==usernameHash;
        }

        public void ResetPassword(ForgotPasswordDto dto, string resetLinkId)
        {
            try
            {
                //get the user that wants to reset password
                var userInDb = GetUserByResetLinkId(resetLinkId);

                //check if reset link was clicked before deadline (2 mins)
                if (DateTime.Now - userInDb.ForgotPasswordTime > new TimeSpan(0, 30, 0))
                    throw new Exception(ErrorMessage.LinkExpired);

                //update user password
                userInDb.PasswordHash = HashPassword(dto.Password);
                userInDb.UpdatedAt = DateTime.Now;
                userInDb.PasswordResetId = null;
                userInDb.ForgotPasswordTime = null;

                //persist changes
                _context.SaveChanges();
                return;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }

        }
        
        public async Task<bool> SendPasswordReset(ForgotPasswordDto dto)
        {
            try
            {
                string linkId;
                ApplicationUser user;
                if (dto.role == "1") 
                   user= _contextHelper.GetLaundryByUsername(dto.Username);
                else if (dto.role == "2") 
                    user = _contextHelper.GetLaundryByUsername(dto.Username);
                else
                    user = _contextHelper.GetApplicationUser(dto.Username);
                
                if(user is Employee)
                    linkId=GetResetLink(dto.Username,config["LaundryManangerApi:employeeKey"]);
                else 
                    linkId = GetResetLink(dto.Username, config["LaundryManagerApi:laundryKey"]);
               
                //send the password reset mail
                SendMail(user, linkId);
                return true ;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            
        }

        public LoginResponseDto GetLoginResponse(string username,string password)
        {
            try
            {
                // get application user
                ApplicationUser user=_contextHelper.GetApplicationUser(username);
                //check if password is  changed
                if (user.ForgotPasswordTime != null)
                    throw new Exception(ErrorMessage.PasswordChanged);
                //check if  user password is correct
                if (user.PasswordHash != HashPassword(password ))
                    throw new Exception(ErrorMessage.InCorrectPassword);

                //create the response object if password is correct
                LoginResponseDto resp = new LoginResponseDto{User = user,};

                //get user role if password is correct
                if (user is Employee)
                    resp.UserRole = RoleNames.LaundryEmployee;
                else
                    resp.UserRole = RoleNames.LaundryOwner;
               
                //return response object
                return resp;
            }
            catch(Exception e)
            {
                throw new Exception(e.Message);
            }
            
        }

        public LoginResponseDto GetLoginResponse(string username, string password,string role) 
        {
            try
            {
                ApplicationUser user;
                if (role == RoleNames.LaundryOwner)
                    user = _contextHelper.GetLaundryByUsername(username);
                else if (role == RoleNames.LaundryEmployee)
                    user = _contextHelper.GetEmployeeByUsername(username);
                else
                    throw new Exception(ErrorMessage.EntityDoesNotExist);

                //check if  user password is correct
                if (user.PasswordHash != HashPassword(password))
                    throw new Exception(ErrorMessage.InCorrectPassword);

                //create the response object if password is correct
                LoginResponseDto resp = new LoginResponseDto { User = user, };

                //get user role if password is correct
                if (user is Employee)
                    resp.UserRole = RoleNames.LaundryEmployee;
                else if(user is Laundry)
                    resp.UserRole = RoleNames.LaundryOwner;

                //return response object
                return resp;
            }
            catch(Exception e)
            {
                if (e.Message == ErrorMessage.EntityDoesNotExist)
                    throw new Exception(ErrorMessage.EntityDoesNotExist);
                else if (e.Message == ErrorMessage.InCorrectPassword)
                    throw new Exception(ErrorMessage.InCorrectPassword);
                else
                    throw new Exception(ErrorMessage.FailedDbOperation);
            }
        }

        private async  Task<bool> SendMail(ApplicationUser user,string linkId)
        {
            //update the user data with the password reset link id
            user.PasswordResetId = linkId;
            user.UpdatedAt = DateTime.Now;
            user.ForgotPasswordTime = DateTime.Now;
            _context.SaveChanges();

            //send the user the password reset link
            string url = config["LaundryManagerApi:laundryWebClient"];
            url += $"?id={linkId}";
            string mailContent = $"<p> Hi {user.Name},</p> <p> Please click <a href='{url}'>here</a> to change your password";
           
            await mailService.SendMailAsync(user.Username, mailContent, "Password Reset");
            return true;
        }
        private ApplicationUser GetUserByResetLinkId(string resetLinkId)
        {
            ApplicationUser userInDb;
            //bool name= "lIcAXoLiMvuE21TdBpXb5vnFQj6dLLKVas1dhy7Nu22AvP0w93DgZc=" == ""
            if (resetLinkId.Substring(0, 1) == config["LaundryManagerApi:laundryKey"])
                userInDb = _context.Laundries.SingleOrDefault(x => x.PasswordResetId == resetLinkId);
            else if (resetLinkId.Substring(0, 1) == config["LaundryManagerApi:employeeKey"])
                userInDb = _context.Employees.SingleOrDefault(x => x.PasswordResetId == resetLinkId);
            else
            {
                Laundry laundry = _context.Laundries.SingleOrDefault(x => x.PasswordResetId == resetLinkId);
                Employee employee = _context.Employees.SingleOrDefault(x => x.PasswordResetId == resetLinkId);
                userInDb = (ApplicationUser)laundry ?? employee;
            }
            return userInDb;
        }

    }
}
