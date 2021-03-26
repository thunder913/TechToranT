namespace RestaurantMenuProject.Web.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using RestaurantMenuProject.Data.Models.Enums;
    using RestaurantMenuProject.Services.Data.Contracts;

    public class WaiterController : Controller
    {
        private readonly IOrderService orderService;

        public WaiterController(IOrderService orderService)
        {
            this.orderService = orderService;
        }

        public IActionResult Index()
        {
            var newOrders = this.orderService.GetOrdersWithStatus(ProcessType.Pending);
            return this.View(newOrders);
        }
    }
}
