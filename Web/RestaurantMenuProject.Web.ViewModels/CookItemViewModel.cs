using RestaurantMenuProject.Data.Models.Enums;

namespace RestaurantMenuProject.Web.ViewModels
{
    public class CookItemViewModel
    {
        public string OrderId { get; set; }

        public string FoodId { get; set; }

        public string FoodName { get; set; }

        public int Count { get; set; }
    }
}
