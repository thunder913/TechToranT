namespace RestaurantMenuProject.Services.Data.Contracts
{
    using RestaurantMenuProject.Data.Models;

    public interface IUserService
    {
        public ApplicationUser GetUserById(string id);
    }
}
