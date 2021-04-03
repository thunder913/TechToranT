namespace RestaurantMenuProject.Services.Data.Contracts
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using RestaurantMenuProject.Data.Models;
    using RestaurantMenuProject.Web.ViewModels;

    public interface IDrinkTypeService
    {
        public ICollection<MenuItemViewModel> GetAllDrinkTypes();

        public ICollection<FoodTypeViewModel> GetAllDrinkTypesWithId();

        public DrinkType GetDrinkTypeById(int id);

        public Task AddDrinkTypeAsync(AddCategoryViewModel drinkCategory, string wwwroot);

        public Task EditDrinkTypeAsync(EditCategoryViewModel editCategory, string wwwroot);

        public Task DeleteDrinkTypeAsync(int id);
    }
}
