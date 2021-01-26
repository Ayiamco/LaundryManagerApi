using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using LaundryApi.Models;

namespace LaundryApi.Infrastructure
{
    public class LaundryApiContext: DbContext
    {
        public LaundryApiContext(DbContextOptions<LaundryApiContext> options)
            : base(options)
        {
        }

        public DbSet<Laundry> Laundries { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Employee> Employees { get; set; }

        //public DbSet<Service> Services { get; set; }
        //public DbSet<Invoice> Invoices { get; set; }

        //public DbSet<InvoiceItem> InvoiceItems { get; set; }
        ////

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Laundry>()
                .HasIndex(x => x.Username)
                .IsUnique();

            builder.Entity<Laundry>()
                .HasIndex(x => x.PasswordResetId)
                .IsUnique();

            builder.Entity<Employee>()
                .HasIndex(x => x.Username)
                .IsUnique();

            builder.Entity<Employee>()
                .HasIndex(x => x.PasswordResetId)
                .IsUnique();


            builder.Entity<Customer>()
                .HasAlternateKey(s => new { s.Username, s.EmployeeId });

            //builder.Entity<Service>()
            //    .HasAlternateKey(s => new { s.Description, s.ApplicationUserId });




        }

    }
}
