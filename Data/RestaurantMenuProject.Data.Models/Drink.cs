namespace RestaurantMenuProject.Data.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using RestaurantMenuProject.Data.Common.Models;
    using RestaurantMenuProject.Data.Models.Enums;

    public class Drink : BaseDeletableModel<int>
    {
        [Required]
        public string Name { get; set; }

        public decimal Price { get; set; }

        public double Weight { get; set; }

        public string AdditionalInfo { get; set; }

        public decimal? AlchoholByVolume { get; set; }

        public DrinkType DrinkType { get; set; }

        public PackagingType PackagingType { get; set; }

        public virtual ICollection<Ingredient> Ingredients { get; set; } = new HashSet<Ingredient>();

        public virtual ICollection<OrderDrink> OrderDrinks { get; set; } = new HashSet<OrderDrink>();
    }
}
