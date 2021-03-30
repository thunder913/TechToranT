using RestaurantMenuProject.Data.Common.Models;
using RestaurantMenuProject.Data.Models.Dtos;
using RestaurantMenuProject.Services.Mapping;
using System;

namespace RestaurantMenuProject.Data.Models
{
    public class PickupItem : BaseDeletableModel<string>
    {
        public PickupItem()
        {
            this.Id = Guid.NewGuid().ToString();
        }

        public string OrderId { get; set; }

        public Order Order { get; set; }

        public string Name { get; set; }

        public string ClientName { get; set; }

        public int TableNumber { get; set; }

        public string WaiterId { get; set; }

        public int Count { get; set; }
    }
}
