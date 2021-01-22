using LaundryApi.Dtos;
using LaundryApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static LaundryApi.Infrastructure.HelperMethods;

namespace LaundryApi.Interfaces
{
    public interface ILaundryRepository
    {
        public Task<LaundryDto> CreateAsync(NewLaundryDto newLaundryDto);
        public  Task<LaundryDto> FindAsync(Guid laundryId);
        //public LaundryDto GetLaundryByUsername(string laundryUsername);

   



    }
}
