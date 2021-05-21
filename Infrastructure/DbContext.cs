using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using LaundryApi.Entites;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace LaundryApi.Infrastructure
{
    public class ApplicationDbContext : IdentityDbContext
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
        //    //builder.Entity<Laundry>()
        //    //    .HasIndex(x => x.Username)
        //    //    .IsUnique();

        //    //builder.Entity<Laundry>()
        //    //    .HasIndex(x => x.PasswordResetId)
        //    //    .IsUnique();

        //    //builder.Entity<Employee>()
        //    //    .HasIndex(x => x.Username)
        //    //    .IsUnique();

        //    //builder.Entity<Employee>()
        //    //    .HasIndex(x => x.PasswordResetId)
        //    //    .IsUnique();

        //    //builder.Entity<EmployeeInTransit>()
        //    //   .HasAlternateKey(s => new { s.Email, s.LaundryId });

        //    //builder.Entity<LaundryService>()
        //    //   .HasAlternateKey(s => new { s.Name, s.LaundryId });

        //}

    }
}
