namespace RestaurantMenuProject.Web.Hubs
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.SignalR;
    using RestaurantMenuProject.Common;
    using RestaurantMenuProject.Data.Models;
    using RestaurantMenuProject.Data.Models.Dtos;
    using RestaurantMenuProject.Data.Models.Enums;
    using RestaurantMenuProject.Services.Data.Contracts;
    using RestaurantMenuProject.Web.ViewModels;

    public class OrderHub : Hub
    {
        private readonly IPickupItemService pickupItemService;
        private readonly IOrderService orderService;
        private readonly UserManager<ApplicationUser> userManager;

        public OrderHub(
            IPickupItemService pickupItemService,
            IOrderService orderService,
            UserManager<ApplicationUser> userManager
            )
        {
            this.pickupItemService = pickupItemService;
            this.orderService = orderService;
            this.userManager = userManager;
        }

        public async Task AddPickupItem(CookFinishItemViewModel foodItem)
        {
            foodItem.DishType = (FoodType)Enum.Parse(typeof(FoodType), foodItem.FoodType);
            var pickUpItemId = await this.pickupItemService.AddPickupItemAsync(foodItem);

            var item = this.pickupItemService.GetPickupItemById(pickUpItemId);
            await this.Clients.User(item.WaiterId).SendAsync(
                "NewPickup",
                new { Id = item.Id, Count = item.Count, Name = item.Name, TableNumber = item.TableNumber, ClientName = item.ClientName }
                );
        }

        public async Task FinishPickupItem(string id)
        {
            await this.pickupItemService.DeleteItemAsync(id);
        }

        public async Task AcceptOrder(EditStatusDto editStatus)
        {
            var statusEditted = await this.EditStatusAsync(editStatus);
            if (!statusEditted.Value)
            {
                throw new InvalidOperationException("There was an error changing the status of the order!");
            }

            var user = await this.userManager.GetUserAsync(this.Context.User);

            await this.orderService.AddWaiterToOrderAsync(editStatus.OrderId, user.Id);
            var order = this.orderService.GetOrderInListById(editStatus.OrderId);

            var chefIds = this.userManager.GetUsersInRoleAsync(GlobalConstants.ChefRoleName).Result.Select(x => x.Id);
            await this.Clients.All.SendAsync("NewChefOrder", new
            {
                Date = order.Date,
                Name = order.FullName,
                StatusName = Enum.GetName(typeof(ProcessType), order.Status),
                Price = order.Price,
                OrderId = order.Id,
            });
        }

        public async Task ChefApproveOrder(string orderId)
        {
            await this.orderService.ChangeOrderStatusAsync(ProcessType.InProcess, ProcessType.Cooking, orderId);
        }

        private async Task<ActionResult<bool>> EditStatusAsync(EditStatusDto editStatus)
        {
            var oldProcessingTypeId = (ProcessType)Enum.Parse(typeof(ProcessType), editStatus.OldProcessingType);
            await this.orderService.ChangeOrderStatusAsync(oldProcessingTypeId, (ProcessType)editStatus.NewProcessingTypeId, editStatus.OrderId);
            return true;
        }

    }
}
