using RestaurantMenuProject.Data.Common.Models;
using RestaurantMenuProject.Data.Models.Dtos;
using RestaurantMenuProject.Services.Mapping;

namespace RestaurantMenuProject.Data.Models
{
    public class PickupItem : BaseDeletableModel<string>
    {
        public string Name { get; set; }

        public string ClientName { get; set; }

        public int TableNumber { get; set; }
    }
}
