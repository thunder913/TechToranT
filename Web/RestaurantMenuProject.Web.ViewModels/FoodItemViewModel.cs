namespace RestaurantMenuProject.Web.ViewModels
{
    using System.Collections.Generic;

    using RestaurantMenuProject.Data.Models;

    public class FoodItemViewModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public decimal Price { get; set; }

        public double Weight { get; set; }

        public int? PrepareTime { get; set; }

        public DishType DishType { get; set; }

        public string AdditionalInfo { get; set; }

        public virtual ICollection<IngredientViewModel> Ingredients { get; set; } = new HashSet<IngredientViewModel>();
    }
}
