using LaundryApi.Dtos;
using LaundryApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static LaundryApi.Services.HelperMethods;

namespace LaundryApi.Interfaces
{
    public interface ILaundryRepository
    {
        public  Task<Laundry> Create(NewLaundryDto newLaundry);
        public  Task<Laundry> FindAsync(Guid laundryId);
        public Laundry GetLaundry(string laundryUsername);



    }
}
