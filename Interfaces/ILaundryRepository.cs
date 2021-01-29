using LaundryApi.Dtos;
using LaundryApi.Entites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static LaundryApi.Infrastructure.HelperMethods;

namespace LaundryApi.Interfaces
{
    public interface ILaundryRepository
    {
        public  Task<LaundryDto> FindLaundryAsync(Guid laundryId);
        public Task<LaundryDto> CreateLaundryAsync(NewLaundryDto newLaundryDto);
        public  Task<bool> DeleteLaundry(Guid laundryId);
        public  Task<LaundryDto> UpdateLaundry(LaundryDto laundry);


    }
}
