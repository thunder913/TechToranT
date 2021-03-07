namespace RestaurantMenuProject.Services.Data
{
    using System.Linq;
    using System.Threading.Tasks;

    using RestaurantMenuProject.Data.Common.Repositories;
    using RestaurantMenuProject.Data.Models;
    using RestaurantMenuProject.Services.Data.Contracts;

    public class DishService : IDishService
    {
        private readonly IDeletableEntityRepository<Dish> dishRepository;

        public DishService(IDeletableEntityRepository<Dish> dishRepository)
        {
            this.dishRepository = dishRepository;
        }

        public async Task AddDish(Dish dish)
        {
            await this.dishRepository.AddAsync(dish);
            await this.dishRepository.SaveChangesAsync();
        }

        public void RemoveDish(Dish dish)
        {
            this.dishRepository.Delete(dish);
        }

        public Dish GetById(int id)
        {
            return this.dishRepository.AllAsNoTracking().Where(x => x.Id == id).FirstOrDefault();
        }
    }
}
