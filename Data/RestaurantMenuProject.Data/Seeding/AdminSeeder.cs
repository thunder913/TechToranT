namespace RestaurantMenuProject.Data.Seeding
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Identity;
    using Microsoft.Extensions.DependencyInjection;
    using RestaurantMenuProject.Common;
    using RestaurantMenuProject.Data.Models;

    public class AdminSeeder : ISeeder
    {
        public async Task SeedAsync(
            ApplicationDbContext dbContext,
            IServiceProvider serviceProvider)
        {
            const string email = "admin@admin.com";
            var user = dbContext.Users.FirstOrDefault(x => x.Email == email);
            if (user == null)
            {
                var userToAdd = new ApplicationUser()
                {
                    Email = email,
                    UserName = email,
                };

                var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
                var result = await userManager.CreateAsync(userToAdd, "Test1234");
                if (!result.Succeeded)
                {
                    throw new Exception(string.Join(Environment.NewLine, result.Errors.Select(e => e.Description)));
                }

                var added = await userManager.AddToRoleAsync(userToAdd, GlobalConstants.AdministratorRoleName);
                if (!added.Succeeded)
                {
                    throw new Exception(string.Join(Environment.NewLine, result.Errors.Select(e => e.Description)));
                }
            }
        }
    }
}
