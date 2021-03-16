namespace RestaurantMenuProject.Web.Controllers
{
    using System.Collections.Generic;
    using System.Security.Claims;

    using Microsoft.AspNetCore.Mvc;
    using RestaurantMenuProject.Services.Data.Contracts;
    using RestaurantMenuProject.Web.ViewModels;

    public class BasketController : Controller
    {
        private readonly IBasketService basketService;

        public BasketController(IBasketService basketService)
        {
            this.basketService = basketService;
        }

        public IActionResult Index()
        {
            var userId = this.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var drinks = this.basketService.GetDrinksInUserBasket(userId);
            var dishes = this.basketService.GetDishesInUserBasket(userId);

            var food = new List<BasketItemViewModel>();
            food.AddRange(dishes);
            food.AddRange(drinks);
            return this.View(food);
        }
    }
}
