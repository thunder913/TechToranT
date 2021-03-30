using RestaurantMenuProject.Services.Mapping;

namespace RestaurantMenuProject.Data.Models.Dtos
{
    public class OrderDeliveredItemDto : IMapFrom<OrderDish>, IMapFrom<OrderDrink>
    {
        public int DeliveredCount { get; set; }

        public int Count { get; set; }
    }
}
