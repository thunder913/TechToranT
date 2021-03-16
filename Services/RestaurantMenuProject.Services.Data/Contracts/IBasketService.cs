namespace RestaurantMenuProject.Services.Data.Contracts
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using RestaurantMenuProject.Data.Models.Dtos;
    using RestaurantMenuProject.Web.ViewModels;

    public interface IBasketService
    {
        public Task AddToBasketAsync(AddItemToBasketViewModel itemToAdd, string userId);

        public Task AddBasketDrinkItemAsync(string basketId, int quantity, int drinkId);

        public ICollection<BasketItemViewModel> GetDrinksInUserBasket(string userId);

        public ICollection<BasketItemViewModel> GetDishesInUserBasket(string userId);

        public BasketItemViewModel GetBasketDishItemById(int dishId, string userId); 

        public BasketItemViewModel GetBasketDrinkItemById(int drinkId, string userId);

        public BasketItemViewModel AddQuantityToDrink(int drinkId, string userId, int quantity);

        public BasketItemViewModel AddQuantityToDish(int dishId, string userId, int quantity);

        public BasketItemViewModel RemoveDish(int dishIId, string userId, int quantity = 0);

        public BasketItemViewModel RemoveDrink(int drinkId, string userId, int quantity = 0);

        public decimal GetTotalPrice(string userId);
    }
}
