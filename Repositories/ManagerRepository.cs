﻿using AutoMapper;
using LaundryApi.Dtos;
using LaundryApi.Infrastructure;
using LaundryApi.Interfaces;
using LaundryApi.Models;
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
        public async Task<LaundryDto> CreateLaundryAsync(NewLaundryDto newLaundryDto)
        {
            try
            {
                ApplicationUser user = mapper.Map<ApplicationUser>(newLaundryDto);
                user.PasswordHash = HashPassword(newLaundryDto.Password);
                user.CreatedAt = DateTime.Now;
                user.NoOfCustomers = 0;
                user.Revenue = 0;
                user.NoOfEmployees = 0;
                user.UpdatedAt = DateTime.Now;
                user.ForgotPasswordTime = null;
                user.PasswordResetId = null;


                await _context.ApplicationUsers.AddAsync(user);

                //Assign role to user
                Role role = _context.Roles.SingleOrDefault(x => x.Name == RoleNames.LaundryOwner);
                var userRole = new UserRole() { ApplicationUserId = user.Id, RoleId = role.Id };
                await _context.UserRoles.AddAsync(userRole);

                //complete db transaction 
                await _context.SaveChangesAsync();

                //update laundryDto object
                var laundryDto = mapper.Map<LaundryDto>(user);
                laundryDto.Password = newLaundryDto.Password;
                return laundryDto;
            }
            catch (Exception e)
            {
                if (e.InnerException.ToString().Contains("Cannot insert duplicate key row in object 'dbo.ApplicationUsers'"))
                    throw new Exception(ErrorMessage.UsernameAlreadyExist);

                throw new Exception(ErrorMessage.FailedDbOperation);
            }


        }

        public LoginResponseDto GetUserByUsername(string username)
        {
            try
            {
                LoginResponseDto resp = new LoginResponseDto();
                var laundryInDb = _context.ApplicationUsers.SingleOrDefault(_user => _user.Username == username);
                if (laundryInDb == null)
                    throw new Exception(ErrorMessage.FailedDbOperation);

                var laundryDto = mapper.Map<LaundryDto>(laundryInDb);
                resp.Laundry = laundryDto;
                resp.PasswordHash = laundryInDb.PasswordHash;
                Role role = _context.UserRoles.Include("Role").SingleOrDefault(x => x.ApplicationUserId == laundryInDb.Id).Role;
                resp.UserRole = role.Name;
                return resp;
            }
            catch
            {
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
            try
            {
                //get user
                var userInDb = _context.ApplicationUsers.SingleOrDefault(x => x.PasswordResetId == resetLinkId);

                //check if link was clicked before deadline
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
            catch(Exception e)
            {
                if (e.Message == ErrorMessage.LinkExpired)
                    throw new Exception(ErrorMessage.LinkExpired);

                throw new Exception(ErrorMessage.FailedDbOperation);
            }
           
        }

        public bool SendPasswordReset(string username)
        {

            string linkId=GenerateRandomString(10);
            string userClaim = HashPassword(username);
            linkId = linkId + userClaim;
            try
            {
                //Get the user
                var user = _context.ApplicationUsers.SingleOrDefault(x => x.Username == username);
                if (username == null)
                    throw new Exception(ErrorMessage.UserDoesNotExist);

                //update the user data with the password reset link id
                user.PasswordResetId = linkId;
                user.UpdatedAt = DateTime.Now;
                user.ForgotPasswordTime = DateTime.Now;
                _context.SaveChanges();

                //send the user the password reset link
                string url = $"https://localhost:44322/api/account/forgotpassword/{linkId}";
                string mailContent=$"<p> Hi {user.FullName},</p> <p> Please click <a href='{url}'>here</a> to change your password";
                MailService.SendMail(username, mailContent, "Password Reset");
                return true;
            }
            catch(Exception e)
            {
                if (e.Message == ErrorMessage.UserDoesNotExist)
                    throw new Exception (ErrorMessage.UserDoesNotExist);

                throw new Exception(ErrorMessage.FailedDbOperation);
            }
            
        }

        
        
    }
}