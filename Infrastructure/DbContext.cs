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

        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Service> Services { get; set; }

        //public DbSet<Customer> Customers { get; set; }
        //public DbSet<Invoice> Invoices { get; set; }
        //public DbSet<InvoiceItem> InvoiceItems { get; set; }
        //

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<ApplicationUser>()
                .HasIndex(x => x.Username)
                .IsUnique();

            //builder.Entity<Customer>()
            //   .HasIndex(c => c.Email)
            //   .IsUnique();

            builder.Entity<Service>()
                .HasAlternateKey(s => new { s.Description, s.ApplicationUserId });
        }
       
    }
}
