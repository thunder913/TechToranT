namespace RestaurantMenuProject.Services.Data.Contracts
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using RestaurantMenuProject.Data.Models;
    using RestaurantMenuProject.Web.ViewModels;

    public interface IDishTypeService
    {
        public ICollection<MenuItemViewModel> GetAllDishTypes();

        public ICollection<FoodTypeViewModel> GetAllDishTypesWithId();

        public DishType GetDishTypeById(int id);

        public Task AddDishTypeAsync(AddCategoryViewModel dishCategory, string wwwroot);

        public Task EditDishTypeAsync(EditCategoryViewModel editCategory, string wwwroot);

        public Task DeleteDishTypeAsync(int id);

        public ICollection<DishType> GetAllDishTypesWithIds(int[] ids);
    }
}
