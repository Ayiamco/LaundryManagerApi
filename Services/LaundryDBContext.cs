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
    public class LaundryDBContext: ControllerBase, ILaundryContext
    {
        private readonly LaundryApiContext _context;

        public LaundryDBContext(LaundryApiContext _context)
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
                    CreatedAt=DateTime.Now,
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
    }
}
