namespace RestaurantMenuProject.Web.Controllers
{
    using System.Security.Claims;

    using Microsoft.AspNetCore.Mvc;
    using RestaurantMenuProject.Services.Data.Contracts;

    public class OrderController : Controller
    {
        private readonly IOrderService orderService;

        public OrderController(IOrderService orderService)
        {
            this.orderService = orderService;
        }

        public IActionResult Index()
        {
            var userId = this.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var userOrders = this.orderService.GetOrderViewModelsByUserId(userId);
            return this.View(userOrders);
        }

        [HttpPost]
        public IActionResult MakeOrder()
        {
            var userId = this.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            this.orderService.MakeOrder(userId);
            return this.RedirectToAction("Index", "Menu");
        }
    }
}
