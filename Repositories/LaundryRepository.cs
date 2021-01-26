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
        
        public async Task<LaundryDto> FindLaundryAsync(Guid id)
        {
            try
            {
                var laundry = await _context.Laundries.FindAsync(id);
                if (laundry == null)
                    throw new Exception(ErrorMessage.UserDoesNotExist);

                var laundryDto = mapper.Map<LaundryDto>(laundry);
                return laundryDto;
            }
            catch(Exception e)
            {
                if (e.Message == ErrorMessage.UserDoesNotExist)
                    throw new Exception(ErrorMessage.UserDoesNotExist);

                throw new Exception(ErrorMessage.FailedDbOperation);
            }

        }

        public async Task<LaundryDto> CreateLaundryAsync(NewLaundryDto newLaundryDto)
        {
            try
            {
                Laundry user = mapper.Map<Laundry>(newLaundryDto);
                user.PasswordHash = HashPassword(newLaundryDto.Password);
                user.CreatedAt = DateTime.Now;
                user.NoOfCustomers = 0;
                user.Revenue = 0;
                user.NoOfEmployees = 0;
                user.UpdatedAt = DateTime.Now;
                user.ForgotPasswordTime = null;
                user.PasswordResetId = null;


                await _context.Laundries.AddAsync(user);

                ////Assign role to user
                //Role role = _context.Roles.SingleOrDefault(x => x.Name == RoleNames.LaundryOwner);
                //var userRole = new UserRole() { ApplicationUserId = user.Id, RoleId = role.Id };
                //await _context.UsersRoles.AddAsync(userRole);

                //complete db transaction 
                await _context.SaveChangesAsync();

                //update laundryDto object
                var laundryDto = mapper.Map<LaundryDto>(user);

                return laundryDto;
            }
            catch (Exception e)
            {
                if (e.InnerException.ToString().Contains("Cannot insert duplicate key row in object 'dbo.Laundries'"))
                    throw new Exception(ErrorMessage.UsernameAlreadyExist);

                throw new Exception(ErrorMessage.FailedDbOperation);
            }


        }


    }
}

