using Microsoft.AspNetCore.Mvc;
using RestaurantMenuProject.Services.Data.Contracts;
using System.Security.Claims;

namespace RestaurantMenuProject.Web.Controllers
{
    public class ChefController : Controller
    {
        private readonly IOrderService orderService;

        public ChefController(
            IOrderService orderService
            )
        {
            this.orderService = orderService;
        }

        public IActionResult Index()
        {
            var chefViewModel = this.orderService.GetChefViewModel();

            return this.View(chefViewModel);
        }
    }
}
