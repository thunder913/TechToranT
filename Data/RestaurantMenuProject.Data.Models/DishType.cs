namespace RestaurantMenuProject.Data.Models
{
    using System.Collections.Generic;

    using RestaurantMenuProject.Data.Common.Models;

    public class DishType : BaseDeletableModel<int>
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public string Image { get; set; }

        public ICollection<Dish> Dishes { get; set; } = new HashSet<Dish>();

        public ICollection<PromoCode> PromoCodes { get; set; } = new HashSet<PromoCode>();
    }
}
