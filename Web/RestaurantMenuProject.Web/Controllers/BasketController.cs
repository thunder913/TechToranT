namespace RestaurantMenuProject.Web.Controllers
{
    using System.Collections.Generic;
    using System.Security.Claims;
    using Microsoft.AspNetCore.Authorization;
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

        [Authorize]
        public IActionResult Index()
        {
            var userId = this.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var drinks = this.basketService.GetDrinksInUserBasket(userId);
            var dishes = this.basketService.GetDishesInUserBasket(userId);

            var basketViewModel = new BasketViewModel();
            var food = new List<FoodItemViewModel>();
            food.AddRange(dishes);
            food.AddRange(drinks);

            basketViewModel.Foods = food;
            basketViewModel.PromoCode = this.basketService.GetBasketPromoCodeById(userId);
            return this.View(basketViewModel);
        }
    }
}
