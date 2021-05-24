using LaundryApi.Entites;
using LaundryApi.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LaundryApi.Infrastructure
{
    public class SeedData
    {
        public async static Task EnsurePopulated(IApplicationBuilder app)
        {
            var context = app.ApplicationServices.CreateScope().ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var userManager = app.ApplicationServices.CreateScope().ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = app.ApplicationServices.CreateScope().ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            if (context.Database.GetPendingMigrations().Any())  context.Database.Migrate();

            if(!context.Roles.Any())
            {
                await roleManager.CreateAsync(new IdentityRole(RoleNames.Admin));
                await roleManager.CreateAsync(new IdentityRole(RoleNames.LaundryEmployee));
                await roleManager.CreateAsync(new IdentityRole(RoleNames.LaundryOwner));

                var user= new ApplicationUser { UserName = "admin@gmail.com",Email="admin@gmail.com" };
                await userManager.CreateAsync(user, "Admin*123");
                await userManager.AddToRoleAsync(user, RoleNames.Admin);
            }
           
        }

    }
}
