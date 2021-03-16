namespace RestaurantMenuProject.Services.Data
{
    using System.Linq;

    using RestaurantMenuProject.Data.Common.Repositories;
    using RestaurantMenuProject.Data.Models;
    using RestaurantMenuProject.Services.Data.Contracts;

    public class UserService : IUserService
    {
        private readonly IDeletableEntityRepository<ApplicationUser> userRepository;

        public UserService(IDeletableEntityRepository<ApplicationUser> userRepository)
        {
            this.userRepository = userRepository;
        }

        public ApplicationUser GetUserById(string id)
        {
            return this.userRepository
                        .All()
                        .FirstOrDefault(x => x.Id == id);
        }
    }
}
