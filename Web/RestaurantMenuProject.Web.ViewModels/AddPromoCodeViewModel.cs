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
        [Range(1, int.MaxValue, ErrorMessage = "The maximum usage times cannot be less than 0!")]
        [Display(Name = "Max usage times")]
        public int MaxUsageTimes { get; set; }

        [Display(Name = "Expiration date")]
        public DateTime ExpirationDate { get; set; }

        [Range(0, 100, ErrorMessage = "The promotional percent must be between 0 and 100!")]
        [Display(Name = "Promotion percent")]
        public int PromoPercent { get; set; }

        [DisplayName("Dish categories in the promo")]
        public List<int> ValidDishCategoriesId { get; set; } = new List<int>();

        public List<SelectListItem> ValidDishCategories { get; set; } = new List<SelectListItem>();

        [DisplayName("Drink categories in the promo")]
        public List<int> ValidDrinkCategoriesId { get; set; } = new List<int>();

        public List<SelectListItem> ValidDrinkCategories { get; set; } = new List<SelectListItem>();
    }
}
