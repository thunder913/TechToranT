namespace RestaurantMenuProject.Services.Data.Contracts
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using RestaurantMenuProject.Data.Models;
    using RestaurantMenuProject.Web.ViewModels;

    public interface IDrinkService
    {
        public Task AddDrinkAsync(AddDrinkViewModel drink, string wwwroot);

        public DrinkItemViewModel GetDrinkItemViewModelById(string id);

        public Drink GetDrinkById(string id);

        public ICollection<DrinkItemViewModel> GetAllDrinksByType(string drinkType);

        public EditDrinkViewModel GetEditDrinkViewModelById(string id);

        public Task EditDrinkAsync(EditDrinkViewModel editDrink, string wwwroot);

        public Task DeleteDrinkByIdAsync(string id);

        public Drink GetDrinkWithDeletedById(string id);

        public ICollection<DrinkItemViewModel> GetAllDrinksBySearchTerm(string searchTerm);
    }
}
