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
    public class LaundryRepository: ControllerBase, ILaundryRepository
    {
        private readonly LaundryApiContext _context;
        private readonly IMapper mapper;
        public LaundryRepository(LaundryApiContext _context,IMapper mapper)
        {
            this._context = _context;
            this.mapper = mapper;
        }
        public async Task<LaundryDto> Create(NewLaundryDto newLaundryDto  )
        {
            try
            {
                Laundry newLaundry = new Laundry()
                {
                    Username = newLaundryDto.Username,
                    Password = HashPassword(newLaundryDto.Password),
                    LaundryName = newLaundryDto.LaundryName,
                    CreatedAt = DateTime.Now,
                    PhoneNumber = newLaundryDto.PhoneNumber,
                    Address=newLaundryDto.Address,
                    NoOfCustomers=0,
                    TotalRevenue=0,
                };
                
                await _context.Laundries.AddAsync(newLaundry);
                await _context.SaveChangesAsync();

                var laundryDto=mapper.Map<LaundryDto>(newLaundry);
                return laundryDto;
            }
            catch
            {
                return null;
            }
           
            
        }

        public async Task<LaundryDto> FindAsync (Guid id)
        {
            try
            {
                var laundry = await _context.Laundries.FindAsync(id);
                if (laundry == null)
                    throw new Exception(ErrorMessage.EntityDoesNotExist);

                var laundryDto =mapper.Map<LaundryDto>(laundry);
                return laundryDto;
            }
            catch
            {
                throw new Exception(ErrorMessage.FailedDbOperation);
            }
          
        }

        public LaundryDto GetLaundryByUsername(string laundryUsername)
        {
            try
            {
                var laundryInDb = _context.Laundries.SingleOrDefault(_user => _user.Username == laundryUsername);
                if (laundryInDb == null)
                    throw new Exception(ErrorMessage.FailedDbOperation);

                var laundryDto = mapper.Map<LaundryDto>(laundryInDb);
                return laundryDto;
            }
            catch
            {
                throw new Exception(ErrorMessage.FailedDbOperation);
            }
            
            
        }
    }
}
