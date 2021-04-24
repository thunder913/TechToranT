namespace RestaurantMenuProject.Services.Data.Contracts
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using RestaurantMenuProject.Data.Models;
    using RestaurantMenuProject.Web.ViewModels;

    public interface IDishService
    {
        public Task AddDishAsync(AddDishViewModel dish, string wwwroot);

        public EditDishViewModel GetEditDishViewModelById(string id);

        public Dish GetDishById(string id);

        public Dish GetDishWithDeletedById(string id);

        public DishViewModel GetDishAsFoodItemById(string id);

        public ICollection<DishViewModel> GetAllDisheshWithDishTypeAsFoodItem(string dishType);

        public Task EditDishAsync(EditDishViewModel editDish, string wwwroot);

        public Task DeleteDishByIdAsync(string id);

        public ICollection<DishViewModel> GetDishViewModelBySearchTerm(string searchTerm);
    }
}
