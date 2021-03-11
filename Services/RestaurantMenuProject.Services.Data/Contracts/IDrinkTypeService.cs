namespace RestaurantMenuProject.Services.Data.Contracts
{
    using System.Collections.Generic;

    using RestaurantMenuProject.Data.Models;
    using RestaurantMenuProject.Web.ViewModels;

    public interface IDrinkTypeService
    {
        public ICollection<MenuItemViewModel> GetAllDrinkTypes();

        public ICollection<FoodTypeViewModel> GetAllDrinkTypesWithId();

        public ICollection<DrinkItemViewModel> GetAllDrinksByType(string drinkType);

        public DrinkItemViewModel GetDrinkById(int id);
    }
}
