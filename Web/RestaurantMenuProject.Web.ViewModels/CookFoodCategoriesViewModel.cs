namespace RestaurantMenuProject.Web.ViewModels
{
    using System.Collections.Generic;

    using RestaurantMenuProject.Data.Models.Enums;

    public class CookFoodCategoriesViewModel
    {
        public FoodType FoodType { get; set; }

        public string CategoryName { get; set; }

        public ICollection<CookItemViewModel> ItemsToCook { get; set; } = new HashSet<CookItemViewModel>();
    }
}
