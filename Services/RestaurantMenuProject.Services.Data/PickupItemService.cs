using RestaurantMenuProject.Data.Common.Repositories;
using RestaurantMenuProject.Data.Models;
using RestaurantMenuProject.Data.Models.Dtos;
using RestaurantMenuProject.Services.Data.Contracts;
using RestaurantMenuProject.Services.Mapping;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;
using RestaurantMenuProject.Web.ViewModels;
using RestaurantMenuProject.Data.Models.Enums;

namespace RestaurantMenuProject.Services.Data
{
    public class PickupItemService : IPickupItemService
    {
        private readonly IDeletableEntityRepository<PickupItem> pickupItemRepository;
        private readonly IOrderService orderService;

        public PickupItemService(
            IDeletableEntityRepository<PickupItem> pickupItemRepository,
            IOrderService orderService)
        {
            this.pickupItemRepository = pickupItemRepository;
            this.orderService = orderService;
        }

        public ICollection<PickupItem> GetAllItemsToPickUp()
        {
            return this.pickupItemRepository.All().ToList();
        }

        public async Task DeleteItemAsync(string id)
        {
            var item = this.pickupItemRepository.All().FirstOrDefault(x => x.Id == id);
            if (item == null)
            {
                throw new InvalidOperationException("Pickup item not found!");
            }

            this.pickupItemRepository.Delete(item);
            await this.pickupItemRepository.SaveChangesAsync();
        }

        public async Task<bool> AddPickupItemAsync(CookFinishItemViewModel viewModel)
        {
            var pickupItem = new PickupItem();

            if (viewModel.DishType == FoodType.Dish)
            {
                await this.orderService.AddDeliveredCountToOrderDish(1, viewModel);
                pickupItem = this.orderService.GetOrderDishAsPickupItem(viewModel);
            }
            else if (viewModel.DishType == FoodType.Drink)
            {
                await this.orderService.AddDeliveredCountToOrderDrink(1, viewModel);
                pickupItem = this.orderService.GetOrderDrinkAsPickupItem(viewModel);
            }

            await this.pickupItemRepository.AddAsync(pickupItem);
            await this.pickupItemRepository.SaveChangesAsync();

            return true;
        }
    }
}
