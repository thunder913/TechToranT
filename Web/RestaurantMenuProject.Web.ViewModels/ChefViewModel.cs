namespace RestaurantMenuProject.Web.ViewModels
{
    using System.Collections;
    using System.Collections.Generic;

    public class ChefViewModel
    {
        public ICollection<OrderInListViewModel> NewOrders { get; set; } = new HashSet<OrderInListViewModel>();

        public ICollection<CookFoodCategoriesViewModel> FoodTypes { get; set; } = new HashSet<CookFoodCategoriesViewModel>();
    }
}
