namespace RestaurantMenuProject.Services.Data
{
    using System.Threading.Tasks;

    using RestaurantMenuProject.Data.Common.Repositories;
    using RestaurantMenuProject.Data.Models;
    using RestaurantMenuProject.Services.Data.Contracts;

    public class DrinkService : IDrinkService
    {
        private readonly IDeletableEntityRepository<Drink> drinkRepository;

        public DrinkService(IDeletableEntityRepository<Drink> drinkRepository)
        {
            this.drinkRepository = drinkRepository;
        }

        public async Task AddDrink(Drink drink)
        {
            await this.drinkRepository.AddAsync(drink);
            await this.drinkRepository.SaveChangesAsync();
        }
    }
}
