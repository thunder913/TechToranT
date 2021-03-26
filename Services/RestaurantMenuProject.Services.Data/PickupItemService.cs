using RestaurantMenuProject.Data.Common.Repositories;
using RestaurantMenuProject.Data.Models;
using RestaurantMenuProject.Data.Models.Dtos;
using RestaurantMenuProject.Services.Data.Contracts;
using RestaurantMenuProject.Services.Mapping;
using System.Collections.Generic;
using System.Linq;

namespace RestaurantMenuProject.Services.Data
{
    public class PickupItemService : IPickupItemService
    {
        private readonly IDeletableEntityRepository<PickupItem> pickupItemRepository;

        public PickupItemService(IDeletableEntityRepository<PickupItem> pickupItemRepository)
        {
            this.pickupItemRepository = pickupItemRepository;
        }

        public ICollection<PickupItem> GetAllItemsToPickUp()
        {
            return this.pickupItemRepository.All().ToList();
        }
    }
}
