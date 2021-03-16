namespace RestaurantMenuProject.Data.Models
{
    using System.Collections.Generic;

    using RestaurantMenuProject.Data.Common.Models;

    public class Basket : BaseDeletableModel<string>
    {
        public Basket()
        {

        }

        public Basket(ApplicationUser user)
        {
            this.User = user;
            this.Id = user.Id;
        }

        public virtual ApplicationUser User { get; set; }

        public virtual ICollection<BasketDish> Dishes { get; set; } = new HashSet<BasketDish>();

        public virtual ICollection<BasketDrink> Drinks { get; set; } = new HashSet<BasketDrink>();
    }
}
