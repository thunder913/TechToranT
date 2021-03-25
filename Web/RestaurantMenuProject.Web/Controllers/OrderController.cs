namespace RestaurantMenuProject.Web.Controllers
{
    using System.Linq;
    using System.Security.Claims;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using RestaurantMenuProject.Common;
    using RestaurantMenuProject.Data.Models;
    using RestaurantMenuProject.Services.Data.Contracts;
    using RestaurantMenuProject.Web.ViewModels;

    [Authorize]
    public class OrderController : Controller
    {
        private readonly IOrderService orderService;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IUserService userService;

        public OrderController(
            IOrderService orderService,
            UserManager<ApplicationUser> userManager,
            IUserService userService
            )
        {
            this.orderService = orderService;
            this.userManager = userManager;
            this.userService = userService;
        }

        [Route("Order/All/{userId}/{id?}")]
        public IActionResult All(string userId, int id = 1)
        {
            const int itemsPerPage = 10;
            var viewModel = new OrderViewModel()
            {
                Page = id,
                Orders = this.orderService.GetOrderViewModelsByUserId(itemsPerPage, id, userId),
                OrdersPerPage = itemsPerPage,
                OrdersCount = this.orderService.GetUserOrdersCount(userId),
            };

            return this.View(viewModel);
        }

        [HttpPost]
        public IActionResult MakeOrder(string tableCode)
        {
            var userId = this.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            this.orderService.MakeOrder(userId, tableCode);
            return this.RedirectToAction("Index", "Menu");
        }

        public IActionResult Index(string id)
        {
            var orderInfo = this.orderService.GetFullInformationForOrder(id);
            return this.View(orderInfo);
        }
    }
}
