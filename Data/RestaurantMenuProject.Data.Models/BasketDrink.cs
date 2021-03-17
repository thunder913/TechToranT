namespace RestaurantMenuProject.Data.Models
{
    using RestaurantMenuProject.Data.Common.Models;

    public class BasketDrink : BaseModel<int>
    {
        public string BasketId { get; set; }

        public string DrinkId { get; set; }

        public virtual Basket Basket { get; set; }

        public virtual Drink Drink { get; set; }

        public int Quantity { get; set; }
    }
}
