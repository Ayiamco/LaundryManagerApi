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

        //public DbSet<Laundry> Laundries { get; set; }
        //public DbSet<Customer> Customers { get; set; }
        //public DbSet<Employee> Employees { get; set; }
        //public DbSet<EmployeeInTransit> EmployeesInTransit { get; set; }
        //public DbSet<LaundryService> Services { get; set; }
        //public DbSet<Invoice> Invoices { get; set; }
        //public DbSet<InvoiceItem> InvoiceItems { get; set; }
        ////
        //protected override void OnModelCreating(ModelBuilder builder)
        //{
        //    base.OnModelCreating(builder);
        //    SeedUsers(builder);
        //    SeedRoles(builder);
        //    SeedUserRoles(builder);

        //    //    //builder.Entity<EmployeeInTransit>()
        //    //    //   .HasAlternateKey(s => new { s.Email, s.LaundryId });

        //    //    //builder.Entity<LaundryService>()
        //    //    //   .HasAlternateKey(s => new { s.Name, s.LaundryId });
        //}

        private static void SeedUsers(ModelBuilder builder)
        {
           ApplicationUser user = new ApplicationUser()
            {
                Id = "b74ddd14-6340-4840-95c2-db12554843e5",
                UserName = "Admin",
                Email = "admin@gmail.com",
                LockoutEnabled = false,
                PhoneNumber = "1234567890"
            };
           
            PasswordHasher<ApplicationUser> passwordHasher = new PasswordHasher<ApplicationUser>();
            user.PasswordHash = passwordHasher.HashPassword(null, "Admin*123");
            builder.Entity<ApplicationUser>().HasData(user);
        }

        private static void SeedRoles(ModelBuilder builder)
        {
            builder.Entity<IdentityRole>().HasData(
                new IdentityRole() { Id = "fab4fac1-c546-41de-aebc-a14da6895711", Name = RoleNames.Admin, ConcurrencyStamp = "1", NormalizedName = RoleNames.Admin },
                new IdentityRole() { Id = "c7b013f0-5201-4317-abd8-c211f91b7330", Name = RoleNames.LaundryOwner, ConcurrencyStamp = "2", NormalizedName = RoleNames.LaundryOwner}
                );
        }

        private static  void SeedUserRoles(ModelBuilder builder)
        {
            builder.Entity<IdentityUserRole<string>>().HasData(
                new IdentityUserRole<string>() { RoleId = "fab4fac1-c546-41de-aebc-a14da6895711", UserId = "b74ddd14-6340-4840-95c2-db12554843e5" }
                );
        }
    
   

    


}
}
