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

        [Range(0,5000)]
        public decimal Price { get; set; }

        [Range(0,5000)]
        public double Weight { get; set; }

        [MinLength(3)]
        [MaxLength(255)]
        public string AdditionalInfo { get; set; }

        [Range(0,100)]
        public decimal? AlchoholByVolume { get; set; }

        public int DrinkTypeId { get; set; }

        public DrinkType DrinkType { get; set; }

        public PackagingType PackagingType { get; set; }

        public int PackagingTypeId { get; set; }

        public virtual ICollection<Ingredient> Ingredients { get; set; } = new HashSet<Ingredient>();

        public virtual ICollection<OrderDrink> OrderDrinks { get; set; } = new HashSet<OrderDrink>();
    }
}
