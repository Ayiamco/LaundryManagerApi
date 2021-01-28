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

namespace LaundryApi.Repositories
{
    public class ManagerRepository: IManagerRepository
    {
        private readonly LaundryApiContext _context;
        private readonly IMapper mapper;
        private readonly IRepositoryHelper _contextHelper;
        public ManagerRepository(LaundryApiContext _context, IMapper mapper, IRepositoryHelper _contextHelper)
        {
            this.mapper = mapper;
            this._context = _context;
            this._contextHelper = _contextHelper;
        }

        

        public Laundry  GetLaundryByUsername(string username)
        {
            try
            {
                var laundryInDb = _context.Laundries.SingleOrDefault(_user => _user.Username == username);
                
                if (laundryInDb == null)
                    throw new Exception(ErrorMessage.UserDoesNotExist);

                return laundryInDb;
       
            }
            catch(Exception e)
            {
                if (e.Message == ErrorMessage.UserDoesNotExist)
                    throw new Exception(ErrorMessage.UserDoesNotExist);
                throw new Exception(ErrorMessage.FailedDbOperation);
            }


        }

        public Employee GetEmployeeByUsername(string username)
        {
            try
            {
                var employeeInDb = _context.Employees.SingleOrDefault(_user => _user.Username == username);
                if (employeeInDb == null)
                    throw new Exception(ErrorMessage.UserDoesNotExist);

                return employeeInDb;

            }
            catch(Exception e)
            {
                if (e.Message == ErrorMessage.UserDoesNotExist)
                    throw new Exception(ErrorMessage.UserDoesNotExist);

                throw new Exception(ErrorMessage.FailedDbOperation);
            }
        }

        public bool IsPasswordResetLinkValid(string username, string resetLinkId)
        {
            var usernameHash = resetLinkId[10..];
            return HashPassword(username)==usernameHash;
        }

        public void ResetPassword(ForgotPasswordDto dto, string resetLinkId)
        {
            try
            {
                //get the user that wants to reset password
                var userInDb = GetUserByResetLinkId(resetLinkId);

                //check if reset link was clicked before deadline (2 mins)
                if (DateTime.Now - userInDb.ForgotPasswordTime > new TimeSpan(0, 2, 0))
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
                if (e.Message == ErrorMessage.LinkExpired)
                    throw new Exception(ErrorMessage.LinkExpired);

                throw new Exception(ErrorMessage.FailedDbOperation);
            }

        }
        
        public async Task<bool> SendPasswordReset(string username)
        {
            try
            {
                //generate the password reset link Id 
                string linkId=GetResetLink(username);

                //Get the application user
                ApplicationUser user = _contextHelper.GetApplicationUser(username);
          
                //send the password reset mail
                bool resp=await SendMail(user, linkId);
                return resp;
            }
            catch (Exception e)
            {
                if (e.Message == ErrorMessage.UserDoesNotExist)
                    throw new Exception(ErrorMessage.UserDoesNotExist);

                else if (e.Message == ErrorMessage.UserHasTwoRoles)
                    throw new Exception(ErrorMessage.UserHasTwoRoles);

                throw new Exception(ErrorMessage.FailedDbOperation);
            }
            
        }

        public LoginResponseDto GetLoginResponse(string username,string password)
        {
            try
            {
                // get application user
                ApplicationUser user=_contextHelper.GetApplicationUser(username);

                //check if  user password is correct
                if (user.PasswordHash != HashPassword(password))
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
                if (e.Message == ErrorMessage.InCorrectPassword)
                    throw new Exception(ErrorMessage.InCorrectPassword);

                else if (e.Message == ErrorMessage.UserHasTwoRoles)
                    throw new Exception(ErrorMessage.UserHasTwoRoles);

                throw new Exception(ErrorMessage.FailedDbOperation);
            }
            
        }

        public LoginResponseDto GetLoginResponse(string username, string password,string role) 
        {
            try
            {
                ApplicationUser user;
                if (role == RoleNames.LaundryOwner)
                    user = GetLaundryByUsername(username);
                else if (role == RoleNames.LaundryEmployee)
                    user = GetEmployeeByUsername(username);
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
                else
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
            string url = $"https://localhost:44322/api/account/forgotpassword/{linkId}";
            string mailContent = $"<p> Hi {user.Name},</p> <p> Please click <a href='{url}'>here</a> to change your password";
            await MailService.SendMailAsync(user.Username, mailContent, "Password Reset");
            return true;
        }

        

        private ApplicationUser GetUserByResetLinkId(string resetLinkId)
        {
            ApplicationUser userInDb;
            if (resetLinkId.Substring(0, 1) == "l")
                userInDb = _context.Laundries.SingleOrDefault(x => x.PasswordResetId == resetLinkId);
            else if (resetLinkId == "e")
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
