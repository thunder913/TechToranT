namespace RestaurantMenuProject.Data.Seeding
{
    using System;
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
                await userManager.CreateAsync(userToAdd, "123456");
                await userManager.AddToRoleAsync(userToAdd, GlobalConstants.AdministratorRoleName);
            }
        }
    }
}
