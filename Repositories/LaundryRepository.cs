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

            
    }
}

