using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LaundryApi.Interfaces;
using LaundryApi.Dtos;
using LaundryApi.Models;
using static LaundryApi.Infrastructure.HelperMethods;
using Microsoft.AspNetCore.Mvc;
using LaundryApi.Infrastructure;
using AutoMapper;

namespace LaundryApi.Repositories
{
    public class LaundryRepository : ControllerBase, ILaundryRepository
    {
        private readonly LaundryApiContext _context;
        private readonly IMapper mapper;
        public LaundryRepository(LaundryApiContext _context, IMapper mapper)
        {
            this._context = _context;
            this.mapper = mapper;
        }
        public async Task<LaundryDto> CreateAsync(NewLaundryDto newLaundryDto)
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
                user.TempPassword = null;

                //creating user roles
                List<Role> applicationRoles = new List<Role>();
                applicationRoles.Add(new Role() { Name = RoleNames.LaundryOwner });
                applicationRoles.Add( new Role() { Name = RoleNames.LaundryEmployee });
                applicationRoles.Add( new Role() { Name = RoleNames.Admin });
                _context.Roles.AddRange(applicationRoles);
                await _context.ApplicationUsers.AddAsync(user);

                //Assign role to user
                //Role role = _context.Roles.SingleOrDefault(x => x.Name == RoleNames.LaundryOwner);
                //var userRole = new UserRole() { ApplicationUserId=user.Id,RoleId=role.Id};
                //await _context.UserRoles.AddAsync(userRole);
                
                //complete db transaction 
                await _context.SaveChangesAsync();

                //update laundryDto object
                var laundryDto = mapper.Map<LaundryDto>(user);
                laundryDto.Password = newLaundryDto.Password;
                return laundryDto;
            }
            catch
            {
                return null;
            }


        }

        public async Task<LaundryDto> FindAsync(Guid id)
        {
            try
            {
                var laundry = await _context.ApplicationUsers.FindAsync(id);
                if (laundry == null)
                    throw new Exception(ErrorMessage.EntityDoesNotExist);

                var laundryDto = mapper.Map<LaundryDto>(laundry);
                return laundryDto;
            }
            catch
            {
                throw new Exception(ErrorMessage.FailedDbOperation);
            }

            //}

            //public LaundryDto GetLaundryByUsername(string laundryUsername)
            //{
            //    try
            //    {
            //        var laundryInDb = _context.Laundries.SingleOrDefault(_user => _user.Username == laundryUsername);
            //        if (laundryInDb == null)
            //            throw new Exception(ErrorMessage.FailedDbOperation);

            //        var laundryDto = mapper.Map<LaundryDto>(laundryInDb);
            //        return laundryDto;
            //    }
            //    catch
            //    {
            //        throw new Exception(ErrorMessage.FailedDbOperation);
            //    }


            //}
        }
    }
}
