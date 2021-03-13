namespace RestaurantMenuProject.Data.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using RestaurantMenuProject.Data.Common.Models;

    public class Allergen : BaseDeletableModel<int>
    {
        // TODO make name Unique
        [Required]
        public string Name { get; set; }

        public ICollection<Ingredient> Ingredients { get; set; } = new HashSet<Ingredient>();
    }
}
