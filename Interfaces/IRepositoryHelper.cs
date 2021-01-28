using LaundryApi.Entites;
using System;
using System.Collections.Generic;
using System.Linq;
using LaundryApi.Models;
using System.Threading.Tasks;

namespace LaundryApi.Interfaces
{
    public interface IRepositoryHelper
    {
        public ApplicationUser GetApplicationUser(string username);

        public Employee GetEmployeeByUsername(string username);

        public Laundry GetLaundryByUsername(string username);
    }
}
