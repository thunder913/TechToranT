namespace RestaurantMenuProject.Data.Models
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using RestaurantMenuProject.Data.Common.Models;
    using RestaurantMenuProject.Data.Models.Enums;
    using RestaurantMenuProject.Services.Mapping;

    public class Order : BaseDeletableModel<string>, IMapFrom<Basket>
    {
        public Order()
        {
            this.Id = Guid.NewGuid().ToString();
        }

        [Required]
        public ProcessType ProcessType { get; set; }

        public DateTime? DeliveredOn { get; set; }

        public DateTime? PaidOn { get; set; }

        [Required]

        public string ClientId { get; set; }

        public virtual ApplicationUser Client { get; set; }

        public virtual PromoCode PromoCode { get; set; }

        public Table Table { get; set; }

        [Required]
        public int TableId { get; set; }

        public string WaiterId { get; set; }

        public ApplicationUser Waiter { get; set; }

        public virtual ICollection<OrderDish> OrderDishes { get; set; } = new HashSet<OrderDish>();

        public virtual ICollection<OrderDrink> OrderDrinks { get; set; } = new HashSet<OrderDrink>();
    }
}
