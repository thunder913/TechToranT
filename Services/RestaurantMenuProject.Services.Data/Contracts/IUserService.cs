namespace RestaurantMenuProject.Services.Data.Contracts
{
    using RestaurantMenuProject.Data.Models;
    using RestaurantMenuProject.Web.ViewModels;
    using System.Collections.Generic;

    public interface IUserService
    {
        public ApplicationUser GetUserById(string id);

        public ICollection<UserInListDetailViewModel> GetAllUserDetails(int itemsPerPage, int page);

        public int GetUsersCount();
    }
}
