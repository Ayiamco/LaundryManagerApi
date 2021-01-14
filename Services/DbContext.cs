using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using LaundryApi.Models;

namespace LaundryApi.Services
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

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Laundry>()
                .HasIndex(l => l.Username)
                .IsUnique();

            builder.Entity<Customer>()
               .HasIndex(c => c.Email)
               .IsUnique();
        }
        //protected  override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    if (optionsBuilder.IsConfigured)
        //    {
        //        optionsBuilder.UseSqlServer("aspnet5-laundryApi", builder => builder.EnableRetryOnFailure());
        //    }

        //}
    }
}
