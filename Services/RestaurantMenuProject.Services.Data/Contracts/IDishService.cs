namespace RestaurantMenuProject.Services.Data.Contracts
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using RestaurantMenuProject.Data.Models;
    using RestaurantMenuProject.Web.ViewModels;

    public interface IDishService
    {
        public Task AddDish(Dish dish);

        public void RemoveDish(Dish dish);

        public Dish GetDishById(int id);
        
        public FoodItemViewModel GetDishAsFoodItemById(int id);

        public ICollection<FoodItemViewModel> GetAllDisheshWithDishTypeAsFoodItem(string dishType);

    }
}
