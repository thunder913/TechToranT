namespace RestaurantMenuProject.Web.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using RestaurantMenuProject.Common;
    using RestaurantMenuProject.Data.Models.Enums;
    using RestaurantMenuProject.Services.Data.Contracts;
    using System.Security.Claims;

    [Authorize(Roles = GlobalConstants.AdministratorRoleName + "," + GlobalConstants.WaiterRoleName)]
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

            var waiterViewModel = this.orderService.GetWaiterViewModel(userId);
            waiterViewModel.PickupItems = this.pickupItemService.GetAllItemsToPickUp(userId); // Just make the method take userId and return only the waiters orders items

            return this.View(waiterViewModel);
        }
    }
}
