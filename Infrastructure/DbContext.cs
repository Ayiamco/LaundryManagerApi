using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using LaundryApi.Entites;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using LaundryApi.Models;

namespace LaundryApi.Infrastructure
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext (DbContextOptions<ApplicationDbContext > options)
            : base(options)
        {
        }

        public DbSet<Laundry> Laundries { get; set; }
        public DbSet<UserProfle> UserProfiles { get; set; }
        public DbSet<Location> Locations { get; set; }
        //public DbSet<Customer> Customers { get; set; }
        //public DbSet<Employee> Employees { get; set; }
        //public DbSet<EmployeeInTransit> EmployeesInTransit { get; set; }
        //public DbSet<LaundryService> Services { get; set; }
        //public DbSet<Invoice> Invoices { get; set; }
        //public DbSet<InvoiceItem> InvoiceItems { get; set; }
       
    
   

    


}
}
