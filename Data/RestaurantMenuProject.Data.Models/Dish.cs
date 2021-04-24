namespace RestaurantMenuProject.Data.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using RestaurantMenuProject.Data.Common.Models;
    using RestaurantMenuProject.Data.Models.Enums;

    public class Dish : BaseDeletableModel<string>
    {
        public Dish()
        {
            this.Id = Guid.NewGuid().ToString();
        }

        [Required]
        public string Name { get; set; }

        // In Leva
        [Range(0, 5000)]
        public decimal Price { get; set; }

        // In Grams
        [Range(0, 25000)]
        public double Weight { get; set; }

        // In Minutes
        [Range(0, 3 * 60)]
        public int? PrepareTime { get; set; }

        public string ImageId { get; set; }

        public Image Image { get; set; }

        [MinLength(3)]
        [MaxLength(255)]
        public string AdditionalInfo { get; set; }

        public int DishTypeId { get; set; }

        public DishType DishType { get; set; }

        public virtual ICollection<Ingredient> Ingredients { get; set; } = new HashSet<Ingredient>();

        public virtual ICollection<OrderDish> OrderDishes { get; set; } = new HashSet<OrderDish>();

        public virtual ICollection<Comment> Comments { get; set; } = new HashSet<Comment>();
    }
}
