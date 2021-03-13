namespace RestaurantMenuProject.Services.Data.Contracts
{
    using System.Threading.Tasks;

    using RestaurantMenuProject.Data.Models;
    using RestaurantMenuProject.Web.ViewModels;

    public interface IDrinkService
    {
        public Task<Drink> AddDrink(AddDrinkViewModel drink);
    }
}
