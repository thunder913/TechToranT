namespace RestaurantMenuProject.Web.ViewModels
{
    using RestaurantMenuProject.Data.Models.Enums;

    public class CookFinishItemViewModel
    {
        public string OrderId { get; set; }

        public string FoodId { get; set; }

        public FoodType DishType { get; set; }

        public string FoodType { get; set; }
    }
}
