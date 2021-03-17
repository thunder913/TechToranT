namespace RestaurantMenuProject.Services.Data.Contracts
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using RestaurantMenuProject.Data.Models;
    using RestaurantMenuProject.Web.ViewModels;

    public interface IDishService
    {
        public Task AddDish(AddDishViewModel dish, string wwwroot);

        public EditDishViewModel GetEditDishViewModelById(string id);

        public void RemoveDish(Dish dish);

        public Dish GetDishById(string id);

        public FoodItemViewModel GetDishAsFoodItemById(string id);

        public ICollection<FoodItemViewModel> GetAllDisheshWithDishTypeAsFoodItem(string dishType);

        public void EditDish(EditDishViewModel editDish, string wwwroot);
    }
}
