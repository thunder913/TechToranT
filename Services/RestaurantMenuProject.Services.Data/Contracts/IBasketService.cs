namespace RestaurantMenuProject.Services.Data.Contracts
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using RestaurantMenuProject.Data.Models;
    using RestaurantMenuProject.Data.Models.Dtos;
    using RestaurantMenuProject.Web.ViewModels;

    public interface IBasketService
    {
        public Task AddToBasketAsync(AddItemToBasketViewModel itemToAdd, string userId);

        public Task AddBasketDrinkItemAsync(string basketId, int quantity, string drinkId);

        public ICollection<BasketItemViewModel> GetDrinksInUserBasket(string userId);

        public ICollection<BasketItemViewModel> GetDishesInUserBasket(string userId);

        public BasketItemViewModel GetBasketDishItemById(string dishId, string userId); 

        public BasketItemViewModel GetBasketDrinkItemById(string drinkId, string userId);

        public BasketItemViewModel AddQuantityToDrink(string drinkId, string userId, int quantity);

        public BasketItemViewModel AddQuantityToDish(string dishId, string userId, int quantity);

        public BasketItemViewModel RemoveDish(string dishIId, string userId, int quantity = 0);

        public BasketItemViewModel RemoveDrink(string drinkId, string userId, int quantity = 0);

        public decimal GetTotalPrice(string userId);

        public BasketDto GetBasket(string userId);

        public Task RemoveBasketItems(string userId);
    }
}
