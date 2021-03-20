namespace RestaurantMenuProject.Services.Data
{
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.AspNetCore.Identity;
    using RestaurantMenuProject.Data.Common.Repositories;
    using RestaurantMenuProject.Data.Models;
    using RestaurantMenuProject.Services.Data.Contracts;
    using RestaurantMenuProject.Services.Mapping;
    using RestaurantMenuProject.Web.ViewModels;

    public class UserService : IUserService
    {
        private readonly IDeletableEntityRepository<ApplicationUser> userRepository;
        private readonly RoleManager<ApplicationRole> roleManager;

        public UserService(
            IDeletableEntityRepository<ApplicationUser> userRepository,
            RoleManager<ApplicationRole> roleManager)
        {
            this.userRepository = userRepository;
            this.roleManager = roleManager;
        }

        public ApplicationUser GetUserById(string id)
        {
            return this.userRepository
                        .All()
                        .FirstOrDefault(x => x.Id == id);
        }

        public ICollection<UserInListDetailViewModel> GetAllUserDetails(int itemsPerPage, int page)
        {
            var users = this
                .userRepository
                .AllAsNoTrackingWithDeleted()
                .OrderBy(x => x.Email)
                .Skip((page - 1) * itemsPerPage)
                .Take(itemsPerPage)
                .To<UserInListDetailViewModel>()
                .ToList();

            for (int i = 0; i < users.Count(); i++)
            {
                var roles = new List<string>();

                foreach (var roleId in users[i].RoleIds)
                {
                    roles.Add(this.roleManager.FindByIdAsync(roleId).GetAwaiter().GetResult().Name);
                }

                users[i].Roles = roles.ToArray();
            }

            return users;
        }

        public int GetUsersCount()
        {
            return this
                .userRepository
                .AllAsNoTrackingWithDeleted()
                .Count();
        }
    }
}
