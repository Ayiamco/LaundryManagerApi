using LaundryApi.Entites;
using LaundryApi.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LaundryApi.Repositories
{
    public class DashboardRepo
    {
        private readonly LaundryApiContext _context;
        public DashboardRepo(LaundryApiContext context)
        {
            _context = context;
        }

        public  void GetLaundry(string username)
        {
            var laundry=_context.Laundries.SingleOrDefault(x => x.Username == username);
            var employeeCount = _context.Employees.Where(x => x.LaundryId == laundry.Id).Count();
            var invoices = _context.Invoices.Where(x => x.LaundryId == laundry.Id).ToList();
            
        }
    }
}
