namespace RestaurantMenuProject.Services.Data.Contracts
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using RestaurantMenuProject.Data.Models;
    using RestaurantMenuProject.Web.ViewModels;

    public interface IDrinkService
    {
        public Task<Drink> AddDrink(AddDrinkViewModel drink, string wwwroot);

        public DrinkItemViewModel GetDrinkItemViewModelById(int id);

        public Drink GetDrinkById(int id);

        public ICollection<DrinkItemViewModel> GetAllDrinksByType(string drinkType);
    }
}
