using AutoMapper;
using LaundryApi.Dtos;
using LaundryApi.Infrastructure;
using LaundryApi.Interfaces;
using LaundryApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using static LaundryApi.Infrastructure.HelperMethods;

namespace LaundryApi.Repositories
{
    public class LaundryRepository  : ControllerBase, ILaundryRepository
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
                Role role= _context.UserRoles.Include("Role").SingleOrDefault(x => x.ApplicationUserId == laundryInDb.Id).Role;
                resp.UserRole = role.Name;
                    return resp;
                }
                catch
                {
                    throw new Exception(ErrorMessage.FailedDbOperation);
                }


            }
    }
}

