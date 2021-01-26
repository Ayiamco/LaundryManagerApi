using AutoMapper;
using LaundryApi.Dtos;
using LaundryApi.Infrastructure;
using LaundryApi.Interfaces;
using LaundryApi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using static LaundryApi.Infrastructure.HelperMethods;

namespace LaundryApi.Repositories
{
    public class ManagerRepository: IManagerRepository
    {
        private readonly LaundryApiContext _context;
        private readonly IMapper mapper;
        public ManagerRepository(LaundryApiContext _context, IMapper mapper)
        {
            this.mapper = mapper;
            this._context = _context;
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
            var usernameHash = resetLinkId.Substring(10);
            return HashPassword(username)==usernameHash;
        }

        public void ResetPassword(ForgotPasswordDto dto, string resetLinkId)
        {
            //try
            //{
            //    //get user
            //    var userInDb = _context.ApplicationUsers.SingleOrDefault(x => x.PasswordResetId == resetLinkId);

            //    //check if link was clicked before deadline
            //    if (DateTime.Now - userInDb.ForgotPasswordTime > new TimeSpan(0, 2, 0))
            //        throw new Exception(ErrorMessage.LinkExpired);

            //    //update user password
            //    userInDb.PasswordHash = HashPassword(dto.Password);
            //    userInDb.UpdatedAt = DateTime.Now;
            //    userInDb.PasswordResetId = null;
            //    userInDb.ForgotPasswordTime = null;

            //    //persist changes
            //    _context.SaveChanges();
            //    return;
            //}
            //catch(Exception e)
            //{
            //    if (e.Message == ErrorMessage.LinkExpired)
            //        throw new Exception(ErrorMessage.LinkExpired);

            //    throw new Exception(ErrorMessage.FailedDbOperation);
            //}
           
        }

        public async Task<bool> SendPasswordReset(string username)
        {

            //string linkId=GenerateRandomString(10);
            //string userClaim = HashPassword(username);
            //linkId = linkId + userClaim;
            //try
            //{
            //    //Get the user
            //    var user = _context.ApplicationUsers.SingleOrDefault(x => x.Username == username);
            //    if (username == null)
            //        throw new Exception(ErrorMessage.UserDoesNotExist);

            //    //update the user data with the password reset link id
            //    user.PasswordResetId = linkId;
            //    user.UpdatedAt = DateTime.Now;
            //    user.ForgotPasswordTime = DateTime.Now;
            //    _context.SaveChanges();

            //    //send the user the password reset link
            //    string url = $"https://localhost:44322/api/account/forgotpassword/{linkId}";
            //    string mailContent=$"<p> Hi {user.FullName},</p> <p> Please click <a href='{url}'>here</a> to change your password";
            //    await MailService.SendMailAsync(username, mailContent, "Password Reset");
            //    return true;
            //}
            //catch(Exception e)
            //{
            //    if (e.Message == ErrorMessage.UserDoesNotExist)
            //        throw new Exception (ErrorMessage.UserDoesNotExist);

            //    throw new Exception(ErrorMessage.FailedDbOperation);
            //}
            return true;
            
        }

        public LoginResponseDto GetLoginResponse(string username,string password)
        {
            try
            {
                Laundry laundry=new Laundry();
                Employee employee= new Employee();

                //get user details by checking the employee and laundry tables
                try {  laundry = GetLaundryByUsername(username); }
                catch(Exception e)
                {
                    if (e.Message == ErrorMessage.UserDoesNotExist)
                        laundry = null;
                    else { }
                }
                try { employee = GetEmployeeByUsername(username); }
                catch (Exception e)
                {
                    if (e.Message == ErrorMessage.UserDoesNotExist)
                        employee = null;
                    else { }
                }

                ApplicationUser user;
                //check if user exist
                if (laundry == null && employee == null)
                    throw new Exception(ErrorMessage.UserDoesNotExist);

                //check if only one user role is tied to the username 
                else if (laundry==null || employee == null)
                {
                    //use null coalesing to get the user
                    user = laundry ?? (ApplicationUser)employee;
                }
                else
                {
                    //if you got to this point username is tied to two user roles
                    throw new Exception(ErrorMessage.UserHasTwoRoles);
                }

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

        
        
    }
}
