namespace RestaurantMenuProject.Data.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using RestaurantMenuProject.Data.Common.Models;
    using RestaurantMenuProject.Data.Models.Enums;

    public class PromoCode : BaseDeletableModel<int>
    {
        [Required]
        public string Code { get; set; }

        [Range(0, int.MaxValue)]
        public int MaxUsageTimes { get; set; }

        public DateTime ExpirationDate { get; set; }

        [Range(0, int.MaxValue)]
        public int UsedTimes { get; set; }

        [Range(0, 100)]
        public int PromoPercent { get; set; }

        public ICollection<Order> Orders { get; set; } = new HashSet<Order>();

        public ICollection<DishType> ValidDishCategories { get; set; } = new HashSet<DishType>();

        public ICollection<DrinkType> ValidDrinkCategories { get; set; } = new HashSet<DrinkType>();
    }
}
