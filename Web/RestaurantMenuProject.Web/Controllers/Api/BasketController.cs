using Microsoft.AspNetCore.Mvc;
using RestaurantMenuProject.Data.Models.Dtos;
using RestaurantMenuProject.Services.Data.Contracts;
using RestaurantMenuProject.Web.ViewModels;
using System.Security.Claims;

namespace RestaurantMenuProject.Web.Controllers.Api
{
    [Route("api/[Controller]")]
    [ApiController]
    public class BasketController : ControllerBase
    {
        private readonly IBasketService basketService;

        public BasketController(IBasketService basketService)
        {
            this.basketService = basketService;
        }

        [HttpPost]
        public void AddItemToBasket(AddItemToBasketViewModel addItem)
        {
            var userId = this.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            this.basketService.AddToBasketAsync(addItem, userId);
        }

        [HttpPost("Add")]
        public BasketItemViewModel AddItem(BasketItemDto basketItem)
        {
            var userId = this.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            switch (basketItem.Type.ToLower())
            {
                case "drink":
                    return this.basketService.AddQuantityToDrink(basketItem.Id, userId, 1);
                case "dish":
                    return this.basketService.AddQuantityToDish(basketItem.Id, userId, 1);
            }

            return null;
        }

        [HttpPost("RemoveAll")]
        public bool RemoveItem(BasketItemDto basketItem)
        {
            var userId = this.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            switch (basketItem.Type.ToLower())
            {
                case "drink":
                    return this.basketService.RemoveDrink(basketItem.Id, userId) == null;
                case "dish":
                    return this.basketService.RemoveDish(basketItem.Id, userId) == null;
            }

            return false;
        }

        [HttpPost("RemoveOne")]
        public BasketItemViewModel RemoveOneItem(BasketItemDto basketItem)
        {
            var userId = this.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            switch (basketItem.Type.ToLower())
            {
                case "drink":
                    return this.basketService.RemoveDrink(basketItem.Id, userId, 1);
                case "dish":
                    return this.basketService.RemoveDish(basketItem.Id, userId, 1);
            }

            return null;
        }

        [HttpGet("GetPrice")]
        public decimal GetTotalPrice()
        {
            var userId = this.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            return this.basketService.GetTotalPrice(userId);
        }
    }
}
