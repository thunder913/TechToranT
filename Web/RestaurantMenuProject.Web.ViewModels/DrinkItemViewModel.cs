using RestaurantMenuProject.Data.Models;
using RestaurantMenuProject.Services.Mapping;
using System.Collections.Generic;

namespace RestaurantMenuProject.Web.ViewModels
{
    public class DrinkItemViewModel : IMapFrom<Drink>
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public decimal Price { get; set; }

        public double Weight { get; set; }

        public string AdditionalInfo { get; set; }

        public decimal? AlchoholByVolume { get; set; }

        public DrinkType DrinkType { get; set; }

        public PackagingType PackagingType { get; set; }

        public virtual ICollection<IngredientViewModel> Ingredients { get; set; } = new HashSet<IngredientViewModel>();
    }
}
