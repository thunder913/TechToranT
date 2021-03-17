namespace RestaurantMenuProject.Web.ViewModels
{
    using System.Collections.Generic;
    using AutoMapper;
    using RestaurantMenuProject.Data.Models;
    using RestaurantMenuProject.Services.Mapping;

    public class FoodItemViewModel : IMapFrom<Dish>
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public decimal Price { get; set; }

        public double Weight { get; set; }

        public int? PrepareTime { get; set; }

        public DishType DishType { get; set; }

        public Image Image { get; set; }

        public string AdditionalInfo { get; set; }

        public virtual ICollection<IngredientViewModel> Ingredients { get; set; } = new HashSet<IngredientViewModel>();
    }
}
