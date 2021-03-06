using RestaurantMenuProject.Data.Common.Models;
using System.Collections.Generic;

namespace RestaurantMenuProject.Data.Models
{
    public class DrinkType : BaseDeletableModel<int>
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public string Image { get; set; }

        public ICollection<Drink> Drinks { get; set; } = new HashSet<Drink>();

        public ICollection<PromoCode> PromoCodes { get; set; } = new HashSet<PromoCode>();
    }
}
