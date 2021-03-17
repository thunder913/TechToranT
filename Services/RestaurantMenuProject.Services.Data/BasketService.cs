namespace RestaurantMenuProject.Services.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using RestaurantMenuProject.Data.Common.Repositories;
    using RestaurantMenuProject.Data.Models;
    using RestaurantMenuProject.Data.Models.Enums;
    using RestaurantMenuProject.Services.Data.Contracts;
    using RestaurantMenuProject.Web.ViewModels;

    public class BasketService : IBasketService
    {
        private readonly IDeletableEntityRepository<Basket> basketRepository;
        private readonly IUserService userService;
        private readonly IRepository<BasketDrink> basketDrinkRepository;
        private readonly IRepository<BasketDish> basketDishRepository;

        public BasketService(
            IDeletableEntityRepository<Basket> basketRepository,
            IUserService userService,
            IRepository<BasketDrink> basketDrinkRepository,
            IRepository<BasketDish> basketDishRepository)
        {
            this.basketRepository = basketRepository;
            this.userService = userService;
            this.basketDrinkRepository = basketDrinkRepository;
            this.basketDishRepository = basketDishRepository;
        }

        // Adding item to basket (doesnt matter if dish or drink). Also creating the basket if non existing
        public Task AddToBasketAsync(AddItemToBasketViewModel itemToAdd, string userId)
        {
            if (!this.basketRepository.AllAsNoTracking().Any(x => x.User.Id == userId))
            {
                this.basketRepository.AddAsync(new Basket()
                {
                    Id = userId,
                    User = this.userService.GetUserById(userId),
                }).GetAwaiter().GetResult();
                this.basketRepository.SaveChangesAsync().GetAwaiter().GetResult();

            }

            if (itemToAdd.Type.ToString().ToLower() == "drink")
            {
                var existingDrink = this.GetBasketDrinkInBasketById(userId, itemToAdd.Id);
                if (existingDrink != null)
                {
                    this.AddQuantityToExistingDrink(existingDrink, itemToAdd.Count).GetAwaiter().GetResult();
                }
                else
                {
                    this.AddBasketDrinkItemAsync(userId, itemToAdd.Count, itemToAdd.Id);

                }
            }
            else if (itemToAdd.Type.ToString().ToLower() == "dish")
            {
                var existingDish = this.GetBasketDishInBasketById(userId, itemToAdd.Id);
                if (existingDish != null)
                {
                    this.AddQuantityToExistingDish(existingDish, itemToAdd.Count).GetAwaiter().GetResult();
                }
                else
                {
                    this.AddBasketDishItemAsync(userId, itemToAdd.Count, itemToAdd.Id);
                }
            }

            return Task.CompletedTask;
        }

        // TODO research why it breaks when async task
        // Adding Drink Item to basket
        public Task AddBasketDrinkItemAsync(string basketId, int quantity, string drinkId)
        {
            var basketDrink = new BasketDrink()
            {
                BasketId = basketId,
                Quantity = quantity,
                DrinkId = drinkId,
            };
            this.basketDrinkRepository.AddAsync(basketDrink).GetAwaiter().GetResult();

            this.basketDrinkRepository.SaveChangesAsync().GetAwaiter().GetResult();

            return Task.CompletedTask;
        }

        // Adding dish item to BasketDishes
        public Task AddBasketDishItemAsync(string basketId, int quantity, string dishId)
        {
            var basketDish = new BasketDish()
            {
                BasketId = basketId,
                Quantity = quantity,
                DishId = dishId,
            };
            this.basketDishRepository.AddAsync(basketDish).GetAwaiter().GetResult();

            this.basketDishRepository.SaveChangesAsync().GetAwaiter().GetResult();

            return Task.CompletedTask;
        }

        // Getting drink item by ids
        public BasketDrink GetBasketDrinkInBasketById(string basketId, string drinkId)
        {
            return this.basketDrinkRepository
                    .All()
                    .FirstOrDefault(x => x.BasketId == basketId && x.DrinkId == drinkId);
        }

        // Adding quantity to existing drink
        public Task AddQuantityToExistingDrink(BasketDrink basketDrink, int quantity)
        {
            basketDrink.Quantity += quantity;
            this.basketDrinkRepository.SaveChangesAsync().GetAwaiter().GetResult();
            return Task.CompletedTask;
        }

        // Get dish item by ids
        public BasketDish GetBasketDishInBasketById(string basketId, string dishId)
        {
            return this.basketDishRepository
                .All()
                .FirstOrDefault(x => x.BasketId == basketId && x.DishId == dishId);
        }

        // Adding quantity to existing dish
        public Task AddQuantityToExistingDish(BasketDish basketDish, int quantity)
        {
            basketDish.Quantity += quantity;
            this.basketDishRepository.SaveChangesAsync().GetAwaiter().GetResult();
            return Task.CompletedTask;
        }

        // Getting all the drinks from the user basket
        public ICollection<BasketItemViewModel> GetDrinksInUserBasket(string userId)
        {
            return this.basketDrinkRepository
                        .AllAsNoTracking()
                        .Where(x => x.Basket.User.Id == userId)
                        .Select(x => new BasketItemViewModel()
                        {
                            Name = x.Drink.Name,
                            Price = x.Drink.Price,
                            Quantity = x.Quantity,
                            FoodType = FoodType.Drink,
                            Id = x.DrinkId,
                            FoodCategory = x.Drink.DrinkType.Name,
                        })
                        .ToList();
        }

        // Getting all the dishes from the user basket
        public ICollection<BasketItemViewModel> GetDishesInUserBasket(string userId)
        {
            return this.basketDishRepository
                        .AllAsNoTracking()
                        .Where(x => x.Basket.User.Id == userId)
                        .Select(x => new BasketItemViewModel()
                        {
                            Name = x.Dish.Name,
                            Price = x.Dish.Price,
                            Quantity = x.Quantity,
                            FoodType = FoodType.Dish,
                            Id = x.DishId,
                            FoodCategory = x.Dish.DishType.Name,
                        })
                        .ToList();
        }

        // Getting a basketDishItem with ids
        public BasketItemViewModel GetBasketDishItemById(string dishId, string userId)
        {
            return this.basketDishRepository
            .AllAsNoTracking()
            .Where(x => x.Basket.User.Id == userId && x.DishId == dishId)
            .Select(x => new BasketItemViewModel()
            {
                Name = x.Dish.Name,
                Price = x.Dish.Price,
                Quantity = x.Quantity,
                FoodType = FoodType.Dish,
                Id = x.DishId,
                FoodCategory = x.Dish.DishType.Name,
            })
            .FirstOrDefault();
        }

        // getting a basketDrinkItem by ids
        public BasketItemViewModel GetBasketDrinkItemById(string drinkId, string userId)
        {
            return this.basketDrinkRepository
             .AllAsNoTracking()
             .Where(x => x.Basket.User.Id == userId && x.DrinkId == drinkId)
             .Select(x => new BasketItemViewModel()
             {
                 Name = x.Drink.Name,
                 Price = x.Drink.Price,
                 Quantity = x.Quantity,
                 FoodType = FoodType.Drink,
                 Id = x.DrinkId,
                 FoodCategory = x.Drink.DrinkType.Name,
             })
             .FirstOrDefault();
        }

        // Adding quantity to drink (by given ids)
        public BasketItemViewModel AddQuantityToDrink(string drinkId, string userId, int quantity)
        {
            var drink = this.basketDrinkRepository
                        .All()
                        .FirstOrDefault(x => x.BasketId == userId && x.DrinkId == drinkId);
            if (drink == null)
            {
                throw new InvalidOperationException("The drink is null!");
            }

            drink.Quantity += quantity;
            this.basketDishRepository.SaveChangesAsync().GetAwaiter().GetResult();

            return this.GetBasketDrinkItemById(drinkId, userId);
        }

        // Adding quantity to dish
        public BasketItemViewModel AddQuantityToDish(string dishId, string userId, int quantity)
        {
            var dish =
                       this.basketDishRepository
                       .All()
                       .FirstOrDefault(x => x.BasketId == userId && x.DishId == dishId);
            if (dish == null)
            {
                throw new InvalidOperationException("The dish is null!");
            }

            dish.Quantity += quantity;
            this.basketDishRepository.SaveChangesAsync().GetAwaiter().GetResult();

            return this.GetBasketDishItemById(dishId, userId);
        }

        // Removing a dish from the basket
        public BasketItemViewModel RemoveDish(string dishIId, string userId, int quantity = 0)
        {
            var dish = this.basketDishRepository
                   .All()
                   .FirstOrDefault(x => x.DishId == dishIId && x.BasketId == userId);
            if (quantity == 0 || dish.Quantity - quantity <= 0)
            {
                this.basketDishRepository.Delete(dish);
                this.basketDishRepository.SaveChangesAsync().GetAwaiter().GetResult();
                return null;
            }

            dish.Quantity -= quantity;
            this.basketDishRepository.SaveChangesAsync().GetAwaiter().GetResult();
            return this.GetBasketDishItemById(dishIId, userId);
        }

        // Removing a drink from the basket
        public BasketItemViewModel RemoveDrink(string drinkId, string userId, int quantity = 0)
        {
            var drink = this.basketDrinkRepository
                   .All()
                   .FirstOrDefault(x => x.DrinkId == drinkId && x.BasketId == userId);
            if (quantity == 0 || drink.Quantity - quantity <= 0)
            {
                this.basketDrinkRepository.Delete(drink);
                this.basketDrinkRepository.SaveChangesAsync().GetAwaiter().GetResult();
                return null;
            }

            drink.Quantity -= quantity;
            this.basketDrinkRepository.SaveChangesAsync().GetAwaiter().GetResult();
            return this.GetBasketDrinkItemById(drinkId, userId);
        }

        // Getting the total price
        public decimal GetTotalPrice(string userId)
        {
            var dishPrice = this
                .basketDishRepository
                .AllAsNoTracking()
                .Where(x => x.BasketId == userId)
                .Sum(x => x.Quantity * x.Dish.Price);

            var drinkPrice = this
                .basketDrinkRepository
                .AllAsNoTracking()
                .Where(x => x.BasketId == userId)
                .Sum(x => x.Quantity * x.Drink.Price);

            return dishPrice + drinkPrice;
        }
    }
}
