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

        public ICollection<FoodItemViewModel> GetDrinksInUserBasket(string userId);

        public ICollection<FoodItemViewModel> GetDishesInUserBasket(string userId);

        public FoodItemViewModel GetBasketDishItemById(string dishId, string userId); 

        public FoodItemViewModel GetBasketDrinkItemById(string drinkId, string userId);

        public Task<FoodItemViewModel> AddQuantityToDrinkAsync(string drinkId, string userId, int quantity);

        public Task<FoodItemViewModel> AddQuantityToDishAsync(string dishId, string userId, int quantity);

        public Task<FoodItemViewModel> RemoveDishAsync(string dishIId, string userId, int quantity = 0);

        public Task<FoodItemViewModel> RemoveDrinkAsync(string drinkId, string userId, int quantity = 0);

        public decimal GetTotalPrice(string userId);

        public BasketDto GetBasket(string userId);

        public Task RemoveBasketItemsAsync(string userId);
    }
}
