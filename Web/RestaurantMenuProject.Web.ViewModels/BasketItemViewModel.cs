namespace RestaurantMenuProject.Web.ViewModels
{
    using RestaurantMenuProject.Data.Models.Enums;

    public class BasketItemViewModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int Quantity { get; set; }

        public decimal Price { get; set; }

        public FoodType FoodType { get; set; }

        public string FoodCategory { get; set; }
    }
}
