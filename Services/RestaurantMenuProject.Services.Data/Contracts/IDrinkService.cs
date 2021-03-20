namespace RestaurantMenuProject.Services.Data.Contracts
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using RestaurantMenuProject.Data.Models;
    using RestaurantMenuProject.Web.ViewModels;

    public interface IDrinkService
    {
        public Task AddDrink(AddDrinkViewModel drink, string wwwroot);

        public DrinkItemViewModel GetDrinkItemViewModelById(string id);

        public Drink GetDrinkById(string id);

        public ICollection<DrinkItemViewModel> GetAllDrinksByType(string drinkType);

        public EditDrinkViewModel GetEditDrinkViewModelById(string id);

        public void EditDrink(EditDrinkViewModel editDrink, string wwwroot);

        public void DeleteDrinkById(string id);

        public Drink GetDrinkWithDeletedById(string id);
    }
}
