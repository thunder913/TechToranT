using RestaurantMenuProject.Web.ViewModels;
using System.Collections.Generic;

namespace RestaurantMenuProject.Services.Data.Contracts
{
    public interface IIngredientService
    {
        public ICollection<DishIngredientViewModel> GetAllAsDishIngredientViewModel();
    }
}
