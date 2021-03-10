namespace RestaurantMenuProject.Web.ViewModels
{
    using System.Collections.Generic;

    public class IngredientViewModel
    {
        public string Name { get; set; }

        public virtual ICollection<AllergenViewModel> Allergens { get; set; } = new HashSet<AllergenViewModel>();

    }
}
