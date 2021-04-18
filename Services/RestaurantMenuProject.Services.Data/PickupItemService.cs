namespace RestaurantMenuProject.Services.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using RestaurantMenuProject.Data.Common.Repositories;
    using RestaurantMenuProject.Data.Models;
    using RestaurantMenuProject.Data.Models.Enums;
    using RestaurantMenuProject.Services.Data.Contracts;
    using RestaurantMenuProject.Web.ViewModels;

    public class PickupItemService : IPickupItemService
    {
        private readonly IDeletableEntityRepository<PickupItem> pickupItemRepository;
        private readonly IOrderService orderService;
        private readonly IOrderDishService orderDishService;
        private readonly IOrderDrinkService orderDrinkService;

        public PickupItemService(
            IDeletableEntityRepository<PickupItem> pickupItemRepository,
            IOrderService orderService,
            IOrderDishService orderDishService,
            IOrderDrinkService orderDrinkService)
        {
            this.pickupItemRepository = pickupItemRepository;
            this.orderService = orderService;
            this.orderDishService = orderDishService;
            this.orderDrinkService = orderDrinkService;
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

        public async Task<string> AddPickupItemAsync(CookFinishItemViewModel viewModel)
        {
            var oldPickupItem = new PickupItem();

            if (viewModel.DishType == FoodType.Dish)
            {
                await this.orderDishService.AddDeliveredCountToOrderDishAsync(viewModel.OrderId, viewModel.FoodId, 1);
                oldPickupItem = this.orderDishService.GetOrderDishAsPickupItem(viewModel.FoodId, viewModel.OrderId);
            }
            else if (viewModel.DishType == FoodType.Drink)
            {
                await this.orderDrinkService.AddDeliveredCountToOrderDrinkAsync(viewModel.OrderId, viewModel.FoodId, 1);
                oldPickupItem = this.orderDrinkService.GetOrderDrinkAsPickupItem(viewModel.FoodId, viewModel.OrderId);
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
                return pickupItem.Id;
            }

            pickupItem = oldPickupItem;

            await this.pickupItemRepository.AddAsync(pickupItem);
            await this.pickupItemRepository.SaveChangesAsync();

            return pickupItem.Id;
        }

        public PickupItem GetPickupItemById(string id)
        {
            return this.pickupItemRepository
                .AllWithDeleted()
                .FirstOrDefault(x => x.Id == id);
        }

        public bool IsOrderFullyDelivered(string id)
        {
            var isOrderCooked = this.orderService.IsOrderCooked(id);
            var hasItemsToPickup = this.pickupItemRepository.All().Any(x => x.OrderId == id);

            return isOrderCooked && !hasItemsToPickup;
        }
    }
}
