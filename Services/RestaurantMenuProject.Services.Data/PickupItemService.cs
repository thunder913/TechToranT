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

        public ICollection<PickupItem> GetAllItemsToPickUp(string userId)
        {
            return this.pickupItemRepository.All().Where(x => x.WaiterId == userId).ToList();
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
            var oldPickupItem = new PickupItem();

            if (viewModel.DishType == FoodType.Dish)
            {
                await this.orderService.AddDeliveredCountToOrderDishAsync(1, viewModel);
                oldPickupItem = this.orderService.GetOrderDishAsPickupItem(viewModel);
            }
            else if (viewModel.DishType == FoodType.Drink)
            {
                await this.orderService.AddDeliveredCountToOrderDrinkAsync(1, viewModel);
                oldPickupItem = this.orderService.GetOrderDrinkAsPickupItem(viewModel);
            }

            var pickupItem = this.pickupItemRepository
                .All()
                .FirstOrDefault(x => x.TableNumber == oldPickupItem.TableNumber
                && x.OrderId == oldPickupItem.OrderId
                && x.ClientName == oldPickupItem.ClientName
                && x.Name == oldPickupItem.Name);

            if (pickupItem != null)
            {
                pickupItem.Count++;
                this.pickupItemRepository.Update(pickupItem);
                await this.pickupItemRepository.SaveChangesAsync();
                return true;
            }

            pickupItem = oldPickupItem;

            await this.pickupItemRepository.AddAsync(pickupItem);
            await this.pickupItemRepository.SaveChangesAsync();

            return true;
        }
    }
}
