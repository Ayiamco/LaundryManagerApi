using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LaundryApi.Interfaces;
using LaundryApi.Dtos;
using LaundryApi.Models;
using static LaundryApi.Services.HelperMethods;
using Microsoft.AspNetCore.Mvc;

namespace LaundryApi.Services
{
    public class LaundryDBService: ControllerBase, ILaundryDbService
    {
        private readonly LaundryApiContext _context;

        public LaundryDBService(LaundryApiContext _context)
        {
            this._context = _context;
        }
        public async Task<Laundry> Create(NewLaundryDto newLaundryDto  )
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
                
                return newLaundry;
            }
            catch
            {
                return null;
            }
           
            
        }

        public async Task<Laundry> FindAsync (Guid id)
        {
           var laundry =await  _context.Laundries.FindAsync(id);
            return laundry;
        }

        public Laundry GetLaundry(string laundryUsername)
        {
            var laundry= _context.Laundries.FirstOrDefault(_user => _user.Username == laundryUsername);
            return laundry;
        }
    }
}
