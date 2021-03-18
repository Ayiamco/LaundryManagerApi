using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LaundryApi.Dtos;
namespace LaundryApi.Interfaces
{
    public interface IDashboardRepo
    {
        public DashboardDto GetDashboardData(string username);
    }
}
