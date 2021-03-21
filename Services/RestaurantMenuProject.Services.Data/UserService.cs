namespace RestaurantMenuProject.Services.Data
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Dynamic.Core;

    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;
    using RestaurantMenuProject.Data.Common.Repositories;
    using RestaurantMenuProject.Data.Models;
    using RestaurantMenuProject.Services.Data.Contracts;
    using RestaurantMenuProject.Services.Mapping;
    using RestaurantMenuProject.Web.ViewModels;

    public class UserService : IUserService
    {
        private readonly IDeletableEntityRepository<ApplicationUser> userRepository;
        private readonly RoleManager<ApplicationRole> roleManager;
        private readonly IDeletableEntityRepository<ApplicationRole> roleRepository;

        public UserService(
            IDeletableEntityRepository<ApplicationUser> userRepository,
            RoleManager<ApplicationRole> roleManager,
            IDeletableEntityRepository<ApplicationRole> roleRepository)
        {
            this.userRepository = userRepository;
            this.roleManager = roleManager;
            this.roleRepository = roleRepository;
        }

        public ApplicationUser GetUserById(string id)
        {
            return this.userRepository
                        .AllWithDeleted()
                        .Include(x => x.Roles)
                        .FirstOrDefault(x => x.Id == id);
        }

        // TODO make it not take all the elements and then sort them
        // TODO fix this kasha
        public IQueryable<UserViewModel> GetUserDataAsQueryable(string sortColumn, string sortDirection, string searchValue)
        {
            var userData = this.GetAllUserDetails();
            if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortDirection)))
            {
                userData = userData.OrderBy(sortColumn + " " + sortDirection);
            }

            if (!string.IsNullOrEmpty(searchValue))
            {
                userData = userData.Where(m => m.Email.Contains(searchValue)
                                            || m.Name.Contains(searchValue)
                                            || m.Roles.Contains(searchValue)
                                            || m.CreatedOn.ToLocalTime().ToString("dd/MM/yyyy, HH:mm:ss").Contains(searchValue)
                                            || m.DeletedOn.ToString().Contains(searchValue)); // TODO make it possible to search for deletedOn
            }

            return userData;
        }

        public IQueryable<UserViewModel> GetAllUserDetails()
        {
            var users = this
                .userRepository
                .AllAsNoTrackingWithDeleted()
                .To<UserWithRolesViewModel>()
                .ToList();

            for (int i = 0; i < users.Count(); i++)
            {
                var roles = new List<string>();

                foreach (var roleId in users[i].RoleIds)
                {
                    roles.Add(this.roleManager.FindByIdAsync(roleId).GetAwaiter().GetResult().Name);
                }

                users[i].Roles = string.Join(", ", roles);
            }

            return users.Select(x => new UserViewModel()
            {
                CreatedOn = x.CreatedOn.ToLocalTime(),
                DeletedOn = x.DeletedOn?.ToLocalTime(),
                Email = x.Email,
                Id = x.Id,
                Name = x.Name,
                Roles = x.Roles,
            }).AsQueryable();
        }

        public int GetUsersCount()
        {
            return this
                .userRepository
                .AllAsNoTrackingWithDeleted()
                .Count();
        }

        public ICollection<ApplicationRole> GetUserRoles()
        {
            return this.roleRepository.AllAsNoTracking().ToList();
        }

        public void EditUserData(EditUserViewModel editUser)
        {
            var user = this.GetUserById(editUser.Id);

            user.FirstName = editUser.FirstName;
            user.LastName = editUser.LastName;
            user.Email = editUser.Email;
            user.PhoneNumber = editUser.PhoneNumber;

            var roles = this.roleRepository
                .All()
                .Where(x => editUser.RoleIds.Contains(x.Id))
                .Select(x => new IdentityUserRole<string>() { RoleId = x.Id, UserId = editUser.Id})
                .ToList();
            user.Roles = roles;
            this.userRepository.SaveChangesAsync().GetAwaiter().GetResult();
        }
    }
}
