namespace RestaurantMenuProject.Web.ViewModels
{
    using System.Collections.Generic;

    public class BasketViewModel
    {
        public ICollection<FoodItemViewModel> Foods { get; set; } = new HashSet<FoodItemViewModel>();

        public BasketPromoCodeViewModel PromoCode { get; set; }
    }
}
