namespace RestaurantMenuProject.Services.Data.Contracts
{
    using RestaurantMenuProject.Data.Models;
    using RestaurantMenuProject.Web.ViewModels;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public interface IUserService
    {
        public ApplicationUser GetUserById(string id);

        public IQueryable<UserViewModel> GetAllUserDetails();

        public int GetUsersCount();

        public ICollection<UserViewModel> GetUserDataAsQueryable(string sortColumn, string sortDirection, string searchValue);

        public ICollection<ApplicationRole> GetUserRoles();

        public Task EditUserDataAsync(EditUserViewModel editUser);

        public Task SetUserNamesAsync(string userId, string firstName, string lastName);

        public Task<bool> DeleteUserAsync(ApplicationUser user);
    }
}
