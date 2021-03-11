using RestaurantMenuProject.Data.Models;
using RestaurantMenuProject.Web.ViewModels;
using System.Collections.Generic;

namespace RestaurantMenuProject.Services.Data.Contracts
{
    public interface IIngredientService
    {
        public ICollection<DishIngredientViewModel> GetAllAsDishIngredientViewModel();

        public ICollection<Ingredient> GetAllIngredientsByIds(int[] ids);
    }
}
