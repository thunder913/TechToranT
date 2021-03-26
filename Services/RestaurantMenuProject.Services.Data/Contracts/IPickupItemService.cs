using RestaurantMenuProject.Data.Models;
using System.Collections.Generic;

namespace RestaurantMenuProject.Services.Data.Contracts
{
    public interface IPickupItemService
    {
        public ICollection<PickupItem> GetAllItemsToPickUp();
    }
}
