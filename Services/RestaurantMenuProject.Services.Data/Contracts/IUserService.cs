namespace RestaurantMenuProject.Services.Data.Contracts
{
    using RestaurantMenuProject.Data.Models;
    using RestaurantMenuProject.Web.ViewModels;
    using System.Collections.Generic;
    using System.Linq;

    public interface IUserService
    {
        public ApplicationUser GetUserById(string id);

        public IQueryable<UserViewModel> GetAllUserDetails();

        public int GetUsersCount();

        public IQueryable<UserViewModel> GetUserDataAsQueryable(string sortColumn, string sortDirection, string searchValue);

        public ICollection<ApplicationRole> GetUserRoles();
    }
}
