namespace RestaurantMenuProject.Services.Data.Contracts
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using RestaurantMenuProject.Data.Models;
    using RestaurantMenuProject.Web.ViewModels;

    public interface IIngredientService
    {
        public ICollection<DishIngredientViewModel> GetAllAsDishIngredientViewModel();

        public ICollection<Ingredient> GetAllIngredientsByIds(int[] ids);

        public Task AddIngredientAsync(AddIngredientViewModel ingredient);

        public Ingredient GetIngredientById(int id);
    }
}
