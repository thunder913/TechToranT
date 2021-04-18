namespace RestaurantMenuProject.Services.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using RestaurantMenuProject.Data.Common.Repositories;
    using RestaurantMenuProject.Data.Models;
    using RestaurantMenuProject.Data.Models.Dtos;
    using RestaurantMenuProject.Data.Models.Enums;
    using RestaurantMenuProject.Services.Data.Contracts;
    using RestaurantMenuProject.Services.Mapping;
    using RestaurantMenuProject.Web.ViewModels;

    public class BasketService : IBasketService
    {
        private readonly IDeletableEntityRepository<Basket> basketRepository;
        private readonly IUserService userService;
        private readonly IRepository<BasketDrink> basketDrinkRepository;
        private readonly IRepository<BasketDish> basketDishRepository;
        private readonly IPromoCodeService promoCodeService;

        public BasketService(
            IDeletableEntityRepository<Basket> basketRepository,
            IUserService userService,
            IRepository<BasketDrink> basketDrinkRepository,
            IRepository<BasketDish> basketDishRepository,
            IPromoCodeService promoCodeService)
        {
            this.basketRepository = basketRepository;
            this.userService = userService;
            this.basketDrinkRepository = basketDrinkRepository;
            this.basketDishRepository = basketDishRepository;
            this.promoCodeService = promoCodeService;
        }

        // Adding item to basket (doesnt matter if dish or drink). Also creating the basket if non existing
        public async Task AddToBasketAsync(AddItemToBasketViewModel itemToAdd, string userId)
        {
            if (!this.basketRepository.AllAsNoTracking().Any(x => x.User.Id == userId))
            {
                await this.basketRepository.AddAsync(new Basket()
                {
                    Id = userId,
                    User = this.userService.GetUserById(userId),
                });
                await this.basketRepository.SaveChangesAsync();

            }

            if (itemToAdd.Type.ToString().ToLower() == "drink")
            {
                var existingDrink = this.GetBasketDrinkInBasketById(userId, itemToAdd.Id);
                if (existingDrink != null)
                {
                    await this.AddQuantityToExistingDrinkAsync(existingDrink, itemToAdd.Count);
                }
                else
                {
                    await this.AddBasketDrinkItemAsync(userId, itemToAdd.Count, itemToAdd.Id);

                }
            }
            else if (itemToAdd.Type.ToString().ToLower() == "dish")
            {
                var existingDish = this.GetBasketDishInBasketById(userId, itemToAdd.Id);
                if (existingDish != null)
                {
                    await this.AddQuantityToExistingDishAsync(existingDish, itemToAdd.Count);
                }
                else
                {
                    await this.AddBasketDishItemAsync(userId, itemToAdd.Count, itemToAdd.Id);
                }
            }
        }

        public async Task AddBasketDrinkItemAsync(string basketId, int quantity, string drinkId)
        {
            var basketDrink = new BasketDrink()
            {
                BasketId = basketId,
                Quantity = quantity,
                DrinkId = drinkId,
            };
            await this.basketDrinkRepository.AddAsync(basketDrink);

            await this.basketDrinkRepository.SaveChangesAsync();

        }

        // Adding dish item to BasketDishes
        public async Task AddBasketDishItemAsync(string basketId, int quantity, string dishId)
        {
            var basketDish = new BasketDish()
            {
                BasketId = basketId,
                Quantity = quantity,
                DishId = dishId,
            };
            await this.basketDishRepository.AddAsync(basketDish);

            await this.basketDishRepository.SaveChangesAsync();
        }

        // Getting drink item by ids
        public BasketDrink GetBasketDrinkInBasketById(string basketId, string drinkId)
        {
            return this.basketDrinkRepository
                    .All()
                    .FirstOrDefault(x => x.BasketId == basketId && x.DrinkId == drinkId);
        }

        // Adding quantity to existing drink
        public async Task AddQuantityToExistingDrinkAsync(BasketDrink basketDrink, int quantity)
        {
            basketDrink.Quantity += quantity;
            await this.basketDrinkRepository.SaveChangesAsync();
        }

        // Get dish item by ids
        public BasketDish GetBasketDishInBasketById(string basketId, string dishId)
        {
            return this.basketDishRepository
                .All()
                .FirstOrDefault(x => x.BasketId == basketId && x.DishId == dishId);
        }

        // Adding quantity to existing dish
        public async Task AddQuantityToExistingDishAsync(BasketDish basketDish, int quantity)
        {
            basketDish.Quantity += quantity;
            await this.basketDishRepository.SaveChangesAsync();
        }

        // Getting all the drinks from the user basket
        public ICollection<FoodItemViewModel> GetDrinksInUserBasket(string userId)
        {
            var drinks = this.basketDrinkRepository
                        .AllAsNoTracking()
                        .Where(x => x.Basket.User.Id == userId)
                        .Select(x => new FoodItemViewModel()
                        {
                            Name = x.Drink.Name,
                            Price = x.Drink.Price,
                            Quantity = x.Quantity,
                            FoodType = FoodType.Drink,
                            Id = x.DrinkId,
                            FoodCategory = x.Drink.DrinkType.Name,
                            Image = x.Drink.Image,
                        })
                        .ToList();

            var promoCode = this.GetPromoCodeInBasket(userId);
            if (promoCode != null)
            {
                foreach (var drink in drinks)
                {
                    if (promoCode.ValidDrinkCategories.Any(x => x.Name == drink.FoodCategory))
                    {
                        drink.Price = Math.Round(drink.Price * (1 - (decimal.Parse(promoCode.PromoPercent.ToString()) / 100)), 2);
                    }
                }
            }

            return drinks;
        }

        // Getting all the dishes from the user basket
        public ICollection<FoodItemViewModel> GetDishesInUserBasket(string userId)
        {
            var dishes = this.basketDishRepository
                        .AllAsNoTracking()
                        .Where(x => x.Basket.User.Id == userId)
                        .Select(x => new FoodItemViewModel()
                        {
                            Name = x.Dish.Name,
                            Price = x.Dish.Price,
                            Quantity = x.Quantity,
                            FoodType = FoodType.Dish,
                            Id = x.DishId,
                            FoodCategory = x.Dish.DishType.Name,
                            Image = x.Dish.Image,
                        })
                        .ToList();


            var promoCode = this.GetPromoCodeInBasket(userId);
            if (promoCode != null)
            {
                foreach (var dish in dishes)
                {
                    if (promoCode.ValidDishCategories.Any(x => x.Name == dish.FoodCategory))
                    {
                        dish.Price = Math.Round(dish.Price * (1 - (decimal.Parse(promoCode.PromoPercent.ToString()) / 100)), 2);
                    }
                }
            }

            return dishes;
        }

        // Getting a basketDishItem with ids
        public FoodItemViewModel GetBasketDishItemById(string dishId, string userId)
        {
            var dish = this.basketDishRepository
            .AllAsNoTracking()
            .Where(x => x.Basket.User.Id == userId && x.DishId == dishId)
            .Select(x => new FoodItemViewModel()
            {
                Name = x.Dish.Name,
                Price = x.Dish.Price,
                Quantity = x.Quantity,
                FoodType = FoodType.Dish,
                Id = x.DishId,
                FoodCategory = x.Dish.DishType.Name,
            })
            .FirstOrDefault();

            var promoCode = this.GetPromoCodeInBasket(userId);
            if (promoCode != null)
            {
                if (promoCode.ValidDishCategories.Any(x => x.Name == dish.FoodCategory))
                {
                    dish.Price = Math.Round(dish.Price * (1 - (decimal.Parse(promoCode.PromoPercent.ToString()) / 100)), 2);
                }
            }

            return dish;
        }

        // getting a basketDrinkItem by ids
        public FoodItemViewModel GetBasketDrinkItemById(string drinkId, string userId)
        {
            var drink = this.basketDrinkRepository
             .AllAsNoTracking()
             .Where(x => x.Basket.User.Id == userId && x.DrinkId == drinkId)
             .Select(x => new FoodItemViewModel()
             {
                 Name = x.Drink.Name,
                 Price = x.Drink.Price,
                 Quantity = x.Quantity,
                 FoodType = FoodType.Drink,
                 Id = x.DrinkId,
                 FoodCategory = x.Drink.DrinkType.Name,
             })
             .FirstOrDefault();

            var promoCode = this.GetPromoCodeInBasket(userId);
            if (promoCode != null)
            {
                if (promoCode.ValidDrinkCategories.Any(x => x.Name == drink.FoodCategory))
                {
                    drink.Price = Math.Round(drink.Price * (1 - (decimal.Parse(promoCode.PromoPercent.ToString()) / 100)), 2);
                }
            }

            return drink;

        }

        // Adding quantity to drink (by given ids)
        public async Task<FoodItemViewModel> AddQuantityToDrinkAsync(string drinkId, string userId, int quantity)
        {
            var drink = this.basketDrinkRepository
                        .All()
                        .FirstOrDefault(x => x.BasketId == userId && x.DrinkId == drinkId);
            if (drink == null)
            {
                throw new InvalidOperationException("The drink is null!");
            }

            drink.Quantity += quantity;
            await this.basketDishRepository.SaveChangesAsync();

            return this.GetBasketDrinkItemById(drinkId, userId);
        }

        // Adding quantity to dish
        public async Task<FoodItemViewModel> AddQuantityToDishAsync(string dishId, string userId, int quantity)
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
            await this.basketDishRepository.SaveChangesAsync();

            return this.GetBasketDishItemById(dishId, userId);
        }

        // Removing a dish from the basket
        public async Task<FoodItemViewModel> RemoveDishAsync(string dishIId, string userId, int quantity = 0)
        {
            var dish = this.basketDishRepository
                   .All()
                   .FirstOrDefault(x => x.DishId == dishIId && x.BasketId == userId);
            if (quantity == 0 || dish.Quantity - quantity <= 0)
            {
                this.basketDishRepository.Delete(dish);
                await this.basketDishRepository.SaveChangesAsync();
                return null;
            }

            dish.Quantity -= quantity;
            await this.basketDishRepository.SaveChangesAsync();
            return this.GetBasketDishItemById(dishIId, userId);
        }

        // Removing a drink from the basket
        public async Task<FoodItemViewModel> RemoveDrinkAsync(string drinkId, string userId, int quantity = 0)
        {
            var drink = this.basketDrinkRepository
                   .All()
                   .FirstOrDefault(x => x.DrinkId == drinkId && x.BasketId == userId);
            if (quantity == 0 || drink.Quantity - quantity <= 0)
            {
                this.basketDrinkRepository.Delete(drink);
                await this.basketDrinkRepository.SaveChangesAsync();
                return null;
            }

            drink.Quantity -= quantity;
            await this.basketDrinkRepository.SaveChangesAsync();
            return this.GetBasketDrinkItemById(drinkId, userId);
        }

        // Getting the total price
        public decimal GetTotalPrice(string userId)
        {
            var dishes = this
                .basketDishRepository
                .AllAsNoTracking()
                .Where(x => x.BasketId == userId)
                .Select(x => new FoodItemPriceDto
                {
                    Price = x.Dish.Price,
                    Category = x.Dish.DishType.Name,
                    Quantity = x.Quantity,
                })
                .ToArray();

            var drinks = this
                .basketDrinkRepository
                .AllAsNoTracking()
                .Where(x => x.BasketId == userId)
                .Select(x => new FoodItemPriceDto
                {
                    Price = x.Drink.Price,
                    Category = x.Drink.DrinkType.Name,
                    Quantity = x.Quantity,
                })
                .ToArray();

            var promoCode = this.GetPromoCodeInBasket(userId);
            if (promoCode != null)
            {
                foreach (var dish in dishes)
                {
                    if (promoCode.ValidDishCategories.Any(x => x.Name == dish.Category))
                    {
                        dish.Price = Math.Round(dish.Price * (1 - (decimal.Parse(promoCode.PromoPercent.ToString()) / 100)), 2);
                    }
                }

                foreach (var drink in drinks)
                {
                    if (promoCode.ValidDrinkCategories.Any(x => x.Name == drink.Category))
                    {
                        drink.Price = Math.Round(drink.Price * (1 - (decimal.Parse(promoCode.PromoPercent.ToString()) / 100)), 2);
                    }
                }
            }

            return dishes.Sum(x => x.Price * x.Quantity) + drinks.Sum(x => x.Price * x.Quantity);
        }

        public BasketDto GetBasket(string userId)
        {
            var basket = this.basketRepository.All().Include(x => x.Dishes).Include(x => x.Drinks).FirstOrDefault(x => x.User.Id == userId);

            return this.basketRepository
                        .All()
                        .Where(x => x.User.Id == userId)
                        .Include(x => x.PromoCode)
                        .ThenInclude(x => x.ValidDishCategories)
                        .Include(x => x.PromoCode)
                        .ThenInclude(x => x.ValidDrinkCategories)
                        .Select(b => new BasketDto()
                        {
                            Id = b.User.Id,
                            Dishes = b.Dishes.Select(d => new FoodCountPriceDto()
                            {
                                Id = d.DishId,
                                Quantity = d.Quantity,
                                Price = d.Dish.Price,
                                CategoryName = d.Dish.DishType.Name,
                            }).ToList(),
                            Drinks = b.Drinks.Select(d => new FoodCountPriceDto()
                            {
                                Id = d.DrinkId,
                                Quantity = d.Quantity,
                                Price = d.Drink.Price,
                                CategoryName = d.Drink.DrinkType.Name,
                            }).ToList(),
                            PromoCode = b.PromoCode,
                        })
                        .First(); // TODO use automapper
        }

        public async Task RemoveBasketItemsAsync(string userId)
        {
            var basketDishes = this.basketDishRepository.All().Where(x => x.Basket.User.Id == userId);
            var basketDrinks = this.basketDrinkRepository.All().Where(x => x.Basket.User.Id == userId);

            foreach (var item in basketDishes)
            {
                this.basketDishRepository.Delete(item);
            }

            foreach (var item in basketDrinks)
            {
                this.basketDrinkRepository.Delete(item);
            }
            var basket = this.basketRepository.All().FirstOrDefault(x => x.Id == userId);
            basket.PromoCode = null;
            await this.basketDishRepository.SaveChangesAsync();
            await this.basketDrinkRepository.SaveChangesAsync();
            await this.basketRepository.SaveChangesAsync();
        }

        public async Task<PromoCode> AddPromoCodeAsync(string code, string userId)
        {
            var promoCode = await this.promoCodeService.GetPromoCodeByCodeAsync(code);
            var basket = this.basketRepository.All().FirstOrDefault(x => x.User.Id == userId);
            basket.PromoCode = promoCode;
            await this.basketRepository.SaveChangesAsync();
            return promoCode;
        }

        public BasketPromoCodeViewModel GetBasketPromoCodeById(string id)
        {
            return this.basketRepository.All()
                .Where(x => x.Id == id)
                .Select(x => new BasketPromoCodeViewModel()
                {
                    Code = x.PromoCode.Code,
                    ExpirationDate = x.PromoCode.ExpirationDate.ToLocalTime().ToString("dd/MM/yyyy HH:mm:ss"),
                })
                .FirstOrDefault();
        }

        public async Task RemovePromoCodeByIdAsync(string id)
        {
            var basket = this.basketRepository.All().Include(x => x.PromoCode).FirstOrDefault(x => x.Id == id);
            basket.PromoCode = null;
            await this.basketRepository.SaveChangesAsync();
        }

        private PromoCode GetPromoCodeInBasket(string id)
        {
            var basket = this.basketRepository.All()
                .Include(x => x.PromoCode.ValidDishCategories)
                .Include(x => x.PromoCode.ValidDrinkCategories)
                .FirstOrDefault(x => x.Id == id);

            if (basket == null)
            {
                return null;
            }

            return basket.PromoCode;
        }
    }
}
