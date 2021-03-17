using RestaurantMenuProject.Data.Common.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RestaurantMenuProject.Data.Models
{
    public class DrinkType : BaseDeletableModel<int>
    {
        public string Name { get; set; }

        public string Description { get; set; }

        [Required]
        public Image Image { get; set; }

        public ICollection<Drink> Drinks { get; set; } = new HashSet<Drink>();

        public ICollection<PromoCode> PromoCodes { get; set; } = new HashSet<PromoCode>();
    }
}
