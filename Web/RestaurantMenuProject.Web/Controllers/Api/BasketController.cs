namespace RestaurantMenuProject.Web.Controllers.Api
{
    using System.Security.Claims;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Mvc;
    using RestaurantMenuProject.Data.Models.Dtos;
    using RestaurantMenuProject.Services.Data.Contracts;
    using RestaurantMenuProject.Web.ViewModels;

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
        public async Task AddItemToBasket(AddItemToBasketViewModel addItem)
        {
            var userId = this.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            await this.basketService.AddToBasketAsync(addItem, userId);
        }

        [HttpPost("Add")]
        public async Task<ActionResult<FoodItemViewModel>> AddItem(BasketItemDto basketItem)
        {
            var userId = this.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            switch (basketItem.Type.ToLower())
            {
                case "drink":
                    return await this.basketService.AddQuantityToDrinkAsync(basketItem.Id, userId, 1);
                case "dish":
                    return await this.basketService.AddQuantityToDishAsync(basketItem.Id, userId, 1);
            }

            return null;
        }

        [HttpPost("RemoveAll")]
        public async Task<ActionResult<bool>> RemoveItem(BasketItemDto basketItem)
        {
            var userId = this.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            switch (basketItem.Type.ToLower())
            {
                case "drink":
                    return await this.basketService.RemoveDrinkAsync(basketItem.Id, userId) == null;
                case "dish":
                    return await this.basketService.RemoveDishAsync(basketItem.Id, userId) == null;
            }

            return false;
        }

        [HttpPost("RemoveOne")]
        public async Task<ActionResult<FoodItemViewModel>> RemoveOneItem(BasketItemDto basketItem)
        {
            var userId = this.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            switch (basketItem.Type.ToLower())
            {
                case "drink":
                    return await this.basketService.RemoveDrinkAsync(basketItem.Id, userId, 1);
                case "dish":
                    return await this.basketService.RemoveDishAsync(basketItem.Id, userId, 1);
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
