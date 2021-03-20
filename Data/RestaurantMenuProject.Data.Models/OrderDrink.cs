namespace RestaurantMenuProject.Data.Models
{
    public class OrderDrink
    {
        public string OrderId { get; set; }

        public Order Order { get; set; }

        public string DrinkId { get; set; }

        public Drink Drink { get; set; }

        public int Count { get; set; }

        public decimal PriceForOne { get; set; }
    }
}
