namespace RestaurantMenuProject.Web.ViewModels
{
    using RestaurantMenuProject.Data.Models;
    using RestaurantMenuProject.Services.Mapping;
    using System.Collections.Generic;

    public class IngredientViewModel : IMapFrom<Ingredient>
    {
        public string Name { get; set; }

        public virtual ICollection<AllergenViewModel> Allergens { get; set; } = new HashSet<AllergenViewModel>();

    }
}
