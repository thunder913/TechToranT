namespace RestaurantMenuProject.Services.Data
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Linq.Dynamic.Core;
    using System.Threading.Tasks;
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

        public ICollection<UserViewModel> GetUserDataAsQueryable(string sortColumn, string sortDirection, string searchValue)
        {
            var userData = this.GetAllUserDetails();

            if (!(string.IsNullOrWhiteSpace(sortColumn) || string.IsNullOrWhiteSpace(sortDirection)))
            {
                userData = userData.OrderBy(sortColumn + " " + sortDirection);
            }

            var dataToReturn = userData.ToList();

            if (!string.IsNullOrWhiteSpace(searchValue))
            {
                dataToReturn = dataToReturn.Where(m => m.Email.ToLower().Contains(searchValue.ToLower())
                                            || m.Name.ToLower().Contains(searchValue.ToLower())
                                            || m.Roles.ToLower().Contains(searchValue.ToLower())
                                            || m.CreatedOn.ToLocalTime().ToString("dd/MM/yyyy, HH:mm:ss").Contains(searchValue)
                                            || m.DeletedOn.ToString().Contains(searchValue)).ToList(); // TODO fix it again to make it do it all as Queryable
            }


            return dataToReturn;
        }

        public IQueryable<UserViewModel> GetAllUserDetails()
        {
                var users = this
                .userRepository
                .AllAsNoTrackingWithDeleted()
                .To<UserWithRolesViewModel>()
                .Select(x => new UserViewModel()
                {
                    CreatedOn = x.CreatedOn,
                    DeletedOn = x.DeletedOn,
                    Email = x.Email,
                    Id = x.Id,
                    Name = x.Name,
                    Roles = string.Join(", ", this.roleRepository.All().Where(y => x.RoleIds.Contains(y.Id))),
                });

                return users;

            // TODO use automapper
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

        public async Task EditUserDataAsync(EditUserViewModel editUser)
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
            await this.userRepository.SaveChangesAsync();
        }

        public async Task SetUserNamesAsync(string userId, string firstName, string lastName)
        {
            var user = this.userRepository.All().FirstOrDefault(x => x.Id == userId);
            if (user.FirstName != firstName)
            {
                user.FirstName = firstName;
            }

            if (user.LastName != lastName)
            {
                user.LastName = lastName;
            }

            await this.userRepository.SaveChangesAsync();
        }

        public async Task<bool> DeleteUserAsync(ApplicationUser user)
        {
            this.userRepository.Delete(user);
            await this.userRepository.SaveChangesAsync();
            return true;
        }

        public async Task<bool> IsUserInTheRole(string userId, string roleName)
        {
            var role = await this.roleManager.FindByNameAsync(roleName);

            var user = this.userRepository.AllAsNoTracking()
                .Include(x => x.Roles)
                .FirstOrDefault(x => x.Id == userId);

            if (user == null)
            {
                return false;
            }

            return user
                .Roles
                .Any(x => role != null && x.RoleId == role.Id);
        }
    }
}
