namespace RestaurantMenuProject.Data.Models
{
    public class OrderDish
    {
        public string OrderId { get; set; }

        public Order Order { get; set; }

        public string DishId { get; set; }

        public Dish Dish { get; set; }

        public int Count { get; set; }

        public decimal PriceForOne { get; set; }

        public int DeliveredCount { get; set; }
    }
}
