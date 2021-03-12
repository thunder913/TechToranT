namespace RestaurantMenuProject.Services.Data.Contracts
{
    using System.Threading.Tasks;

    using RestaurantMenuProject.Data.Models;

    public interface IDrinkService
    {
        public Task AddDrink(Drink drink);
    }
}
