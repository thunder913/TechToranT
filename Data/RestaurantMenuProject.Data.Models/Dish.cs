namespace RestaurantMenuProject.Data.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using RestaurantMenuProject.Data.Common.Models;
    using RestaurantMenuProject.Data.Models.Enums;

    public class Dish : BaseDeletableModel<int>
    {
        [Required]
        public string Name { get; set; }

        public decimal Price { get; set; }

        public double Weight { get; set; }

        // In Minutes
        public int? PrepareTime { get; set; }

        public string AdditionalInfo { get; set; }

        public DishType DishType { get; set; }

        public virtual ICollection<Ingredient> Ingredients { get; set; } = new HashSet<Ingredient>();

        public virtual ICollection<OrderDish> OrderDishes { get; set; } = new HashSet<OrderDish>();
    }
}
