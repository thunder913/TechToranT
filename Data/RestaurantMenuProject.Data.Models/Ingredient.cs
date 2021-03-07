namespace RestaurantMenuProject.Data.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using RestaurantMenuProject.Data.Common.Models;

    public class Ingredient : BaseDeletableModel<int>
    {
        [Required]
        public string Name { get; set; }

        public virtual ICollection<Dish> Dishes { get; set; } = new HashSet<Dish>();

        public virtual ICollection<Drink> Drinks { get; set; } = new HashSet<Drink>();

        public virtual ICollection<Allergen> Allergens { get; set; } = new HashSet<Allergen>();
    }
}
