namespace RestaurantMenuProject.Data.Models
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using RestaurantMenuProject.Data.Common.Models;
    using RestaurantMenuProject.Data.Models.Enums;

    public class Order : BaseDeletableModel<int>
    {
        public decimal TotalPrice { get; set; }

        public ProcessType ProcessType { get; set; }

        public DateTime? DeliveredOn { get; set; }

        public DateTime? PaidOn { get; set; }

        public string ClientId { get; set; }

        public virtual ApplicationUser Client { get; set; }

        public virtual PromoCode PromoCode { get; set; }

        public virtual ICollection<OrderDish> OrderDishes { get; set; } = new HashSet<OrderDish>();

        public virtual ICollection<OrderDrink> OrderDrinks { get; set; } = new HashSet<OrderDrink>();
    }
}
