namespace RestaurantMenuProject.Web.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using RestaurantMenuProject.Data.Models.Enums;
    using RestaurantMenuProject.Services.Data.Contracts;
    using System.Security.Claims;

    public class WaiterController : Controller
    {
        private readonly IOrderService orderService;
        private readonly IPickupItemService pickupItemService;

        public WaiterController(
            IOrderService orderService,
            IPickupItemService pickupItemService)
        {
            this.orderService = orderService;
            this.pickupItemService = pickupItemService;
        }

        public IActionResult Index()
        {
            var userId = this.User.FindFirst(ClaimTypes.NameIdentifier).Value;

            var newOrders = this.orderService.GetWaiterViewModel(userId);
            newOrders.PickupItems = this.pickupItemService.GetAllItemsToPickUp(); // Just make the method take userId and return only the waiters orders items

            return this.View(newOrders);
        }
    }
}
