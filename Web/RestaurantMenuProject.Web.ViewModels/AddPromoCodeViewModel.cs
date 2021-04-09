namespace RestaurantMenuProject.Web.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.Text;

    using Microsoft.AspNetCore.Mvc.Rendering;

    public class AddPromoCodeViewModel
    {
        [Range(0, int.MaxValue)]
        public int MaxUsageTimes { get; set; }

        public DateTime ExpirationDate { get; set; }

        [Range(0, 100)]
        public int PromoPercent { get; set; }

        [DisplayName("Dish categories in the promo")]
        public List<int> ValidDishCategoriesId { get; set; } = new List<int>();

        public List<SelectListItem> ValidDishCategories { get; set; } = new List<SelectListItem>();

        [DisplayName("Drink categories in the promo")]
        public List<int> ValidDrinkCategoriesId { get; set; } = new List<int>();

        public List<SelectListItem> ValidDrinkCategories { get; set; } = new List<SelectListItem>();
    }
}
