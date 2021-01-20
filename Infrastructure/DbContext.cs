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
        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<InvoiceItem> InvoiceItems { get; set; }
        public DbSet<Service> Services { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Laundry>()
                .HasIndex(l => l.Username)
                .IsUnique();

            builder.Entity<Customer>()
               .HasIndex(c => c.Email)
               .IsUnique();

            builder.Entity<Service>()
               .HasIndex(s => s.Description)
               .IsUnique();
        }
       
    }
}
