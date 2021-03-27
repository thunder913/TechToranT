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

        public string Name { get; set; }

        public string ClientName { get; set; }

        public int TableNumber { get; set; }
    }
}
