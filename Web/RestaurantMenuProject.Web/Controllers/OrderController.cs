namespace RestaurantMenuProject.Web.Controllers
{
    using System.Security.Claims;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using RestaurantMenuProject.Services.Data.Contracts;
    using RestaurantMenuProject.Web.ViewModels;

    [Authorize]
    public class OrderController : Controller
    {
        private readonly IOrderService orderService;

        public OrderController(IOrderService orderService)
        {
            this.orderService = orderService;
        }

        public IActionResult All(int id = 1)
        {
            var userId = this.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            const int itemsPerPage = 10;
            var viewModel = new OrderViewModel()
            {
                Page = id,
                Orders = this.orderService.GetOrderViewModelsByUserId(userId, itemsPerPage, id),
                OrdersPerPage = itemsPerPage,
                OrdersCount = this.orderService.GetUserOrdersCount(userId),
            };


            return this.View(viewModel);
        }

        [HttpPost]
        public IActionResult MakeOrder()
        {
            var userId = this.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            this.orderService.MakeOrder(userId);
            return this.RedirectToAction("Index", "Menu");
        }

        public IActionResult Index(string id)
        {
            var orderInfo = this.orderService.GetFullInformationForOrder(id);
            return this.View(orderInfo);
        }
    }
}
