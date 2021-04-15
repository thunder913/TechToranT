namespace RestaurantMenuProject.Services.Data.Tests
{
    using DeepEqual.Syntax;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.DependencyInjection;
    using RestaurantMenuProject.Data.Models;
    using RestaurantMenuProject.Data.Models.Dtos;
    using RestaurantMenuProject.Data.Models.Enums;
    using RestaurantMenuProject.Services.Data.Contracts;
    using RestaurantMenuProject.Web.ViewModels;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Xunit;

    public class BasketServiceTests : BaseServiceTests
    {
        private const string UserId = "1";

        private IBasketService BasketService => this.ServiceProvider.GetRequiredService<IBasketService>();

        [Fact]
        public async Task AddToBasketAsyncWorksCorrectlyWhenAddingDrink()
        {
            await this.AddDrinksToDB();

            var itemToAdd = new AddItemToBasketViewModel()
            {
                Type = "Drink",
                Count = 3,
                Id = "test",
            };

            await this.BasketService.AddToBasketAsync(itemToAdd, "1");

            Assert.Equal(1, this.DbContext.Baskets.Count());

            var itemToAdd2 = new AddItemToBasketViewModel()
            {
                Type = "Drink",
                Count = 3,
                Id = "test1",
            };

            await this.BasketService.AddToBasketAsync(itemToAdd2, UserId);
            var actual = this.DbContext.Baskets.FirstOrDefault(x => x.User.Id == UserId).Drinks;
            Assert.Equal(2, actual.Count());
        }

        [Fact]
        public async Task AddToBasketAsyncWorksCorrectlyWhenAddingExistingDrinks()
        {
            await this.AddDrinksToDB();

            int count = 3;

            var itemToAdd = new AddItemToBasketViewModel()
            {
                Type = "Drink",
                Count = count,
                Id = "test",
            };

            await this.BasketService.AddToBasketAsync(itemToAdd, UserId);

            await this.BasketService.AddToBasketAsync(itemToAdd, UserId);

            Assert.True(this.DbContext.Baskets.FirstOrDefault(x => x.User.Id == UserId).Drinks.Where(x => x.Quantity == count * 2).Any());
        }

        [Fact]
        public async Task AddToBasketAsyncWorksCorrectlyWhenAddingDish()
        {
            await this.AddDishesToDB();

            var itemToAdd = new AddItemToBasketViewModel()
            {
                Type = "Dish",
                Count = 3,
                Id = "test",
            };

            await this.BasketService.AddToBasketAsync(itemToAdd, "1");

            Assert.Equal(1, this.DbContext.Baskets.Count());

            var itemToAdd2 = new AddItemToBasketViewModel()
            {
                Type = "Dish",
                Count = 3,
                Id = "test1",
            };

            await this.BasketService.AddToBasketAsync(itemToAdd2, UserId);
            var actual = this.DbContext.Baskets.FirstOrDefault(x => x.User.Id == UserId).Dishes;
            Assert.Equal(2, actual.Count());
        }

        [Fact]
        public async Task AddToBasketAsyncWorksCorrectlyWhenAddingExistingDish()
        {
            await this.AddDishesToDB();

            int count = 3;

            var itemToAdd = new AddItemToBasketViewModel()
            {
                Type = "Dish",
                Count = count,
                Id = "test",
            };

            await this.BasketService.AddToBasketAsync(itemToAdd, UserId);

            await this.BasketService.AddToBasketAsync(itemToAdd, UserId);

            Assert.True(this.DbContext.Baskets.FirstOrDefault(x => x.User.Id == UserId).Dishes.Where(x => x.Quantity == count * 2).Any());
        }

        [Fact]
        public async Task GetDrinksInUserBasketWorksCorrectlyWithoutPromoCode()
        {
            await this.AddDrinksToDB();
            await this.AddBasketDrinksToUserBasket();


            var expected = this.DbContext.BasketsDrinks
                .Where(x => x.Basket.User.Id == UserId)
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
            var actual = this.BasketService.GetDrinksInUserBasket(UserId);

            actual.ShouldDeepEqual(expected);
        }

        [Fact]
        public async Task GetDrinksInUserBasketWorksCorrectlyWithPromoCode()
        {
            await this.AddDrinksToDB();
            await this.AddBasketDrinksToUserBasket();
            await this.AddPromoCodeToUser();

            var actual = this.BasketService.GetDrinksInUserBasket(UserId);

            var expected = this.DbContext.BasketsDrinks
                .Where(x => x.Basket.User.Id == UserId)
                .Select(x => new FoodItemViewModel()
                {
                    Name = x.Drink.Name,
                    Price = x.Drink.Price * 0.81M,
                    Quantity = x.Quantity,
                    FoodType = FoodType.Drink,
                    Id = x.DrinkId,
                    FoodCategory = x.Drink.DrinkType.Name,
                    Image = x.Drink.Image,
                })
                .ToList();

            actual.ShouldDeepEqual(expected);
        }

        [Fact]
        public async Task GetDishesInUserBasketWorksCorrectlyWithoutPromoCode()
        {
            await this.AddDishesToDB();
            await this.AddBasketDishesToUserBasket();

            var expected = this.DbContext.BasketsDishes
                .Where(x => x.Basket.User.Id == UserId)
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
            var actual = this.BasketService.GetDishesInUserBasket(UserId);

            actual.ShouldDeepEqual(expected);
        }

        [Fact]
        public async Task GetDishesInUserBasketWorksCorrectlyWithPromoCode()
        {
            await this.AddDishesToDB();
            await this.AddBasketDishesToUserBasket();
            await this.AddPromoCodeToUser();

            var actual = this.BasketService.GetDishesInUserBasket(UserId);

            var expected = this.DbContext.BasketsDishes
                .Where(x => x.Basket.User.Id == UserId)
                .Select(x => new FoodItemViewModel()
                {
                    Name = x.Dish.Name,
                    Price = x.Dish.Price * 0.81M,
                    Quantity = x.Quantity,
                    FoodType = FoodType.Dish,
                    Id = x.DishId,
                    FoodCategory = x.Dish.DishType.Name,
                    Image = x.Dish.Image,
                })
                .ToList();

            actual.ShouldDeepEqual(expected);
        }

        [Fact]
        public async Task GetBasketDishItemByIdWorksCorrectlyWithoutPromoCode()
        {
            await this.AddDishesToDB();
            await this.AddBasketDishesToUserBasket();

            var actual = this.DbContext.BasketsDishes
                        .Where(x => x.Basket.User.Id == UserId && x.Dish.Id == "test")
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

            var expected = this.BasketService.GetBasketDishItemById("test", UserId);

            actual.IsDeepEqual(expected);
        }

        [Fact]
        public async Task GetBasketDishItemByIdWorksCorrectlyWithPromoCode()
        {
            await this.AddDishesToDB();
            await this.AddBasketDishesToUserBasket();
            await this.AddPromoCodeToUser();

            var actual = this.DbContext.BasketsDishes
            .Where(x => x.Basket.User.Id == UserId && x.Dish.Id == "test")
                        .Select(x => new FoodItemViewModel()
                        {
                            Name = x.Dish.Name,
                            Price = x.Dish.Price * 0.81M,
                            Quantity = x.Quantity,
                            FoodType = FoodType.Dish,
                            Id = x.DishId,
                            FoodCategory = x.Dish.DishType.Name,
                        })
                        .FirstOrDefault();

            var expected = this.BasketService.GetBasketDishItemById("test", UserId);

            actual.IsDeepEqual(expected);
        }

        [Fact]
        public async Task GetBasketDrinkItemByIdWorksCorrectlyWithoutPromoCode()
        {
            await this.AddDrinksToDB();
            await this.AddBasketDrinksToUserBasket();

            var actual = this.DbContext.BasketsDrinks
                        .Where(x => x.Basket.User.Id == UserId && x.Drink.Id == "test")
                                    .Select(x => new FoodItemViewModel()
                                    {
                                        Name = x.Drink.Name,
                                        Price = x.Drink.Price,
                                        Quantity = x.Quantity,
                                        FoodType = FoodType.Drink,
                                        Id = x.Drink.Id,
                                        FoodCategory = x.Drink.DrinkType.Name,
                                    })
                                    .FirstOrDefault();

            var expected = this.BasketService.GetBasketDrinkItemById("test", UserId);

            actual.IsDeepEqual(expected);
        }

        [Fact]
        public async Task GetBasketDrinkItemByIdWorksCorrectlyWithPromoCode()
        {
            await this.AddDrinksToDB();
            await this.AddBasketDrinksToUserBasket();
            await this.AddPromoCodeToUser();

            var actual = this.DbContext.BasketsDrinks
            .Where(x => x.Basket.User.Id == UserId && x.Drink.Id == "test")
                        .Select(x => new FoodItemViewModel()
                        {
                            Name = x.Drink.Name,
                            Price = x.Drink.Price * 0.81M,
                            Quantity = x.Quantity,
                            FoodType = FoodType.Drink,
                            Id = x.DrinkId,
                            FoodCategory = x.Drink.DrinkType.Name,
                        })
                        .FirstOrDefault();

            var expected = this.BasketService.GetBasketDrinkItemById("test", UserId);

            actual.IsDeepEqual(expected);
        }

        [Fact]
        public async Task AddQuantityToDrinkAsyncWorksCorrectly()
        {
            await this.AddDrinksToDB();
            await this.AddBasketDrinksToUserBasket();

            await this.BasketService.AddQuantityToDrinkAsync("test", UserId, 5);
            var expectedCount = 8;
            var actualCount = this.DbContext.BasketsDrinks.FirstOrDefault(x => x.Basket.User.Id == UserId && x.DrinkId == "test").Quantity;

            Assert.Equal(expectedCount, actualCount);
        }

        [Fact]
        public async Task AddQuantityToDrinkAsyncThrowsExceptionIfThereisNoSuchDrinkId()
        {
            await Assert.ThrowsAsync<InvalidOperationException>(async () => await this.BasketService.AddQuantityToDrinkAsync("invalid", UserId, 30));
        }

        [Fact]
        public async Task AddQuantityToDrinkAsyncThrowsExceptionIfThereisNoSuchUserId()
        {
            await Assert.ThrowsAsync<InvalidOperationException>(async () => await this.BasketService.AddQuantityToDrinkAsync("invalid", "INVALID", 30));
        }

        [Fact]
        public async Task AddQuantityToDishAsyncWorksCorrectly()
        {
            await this.AddDishesToDB();
            await this.AddBasketDishesToUserBasket();

            await this.BasketService.AddQuantityToDishAsync("test", UserId, 5);
            var expectedCount = 8;
            var actualCount = this.DbContext.BasketsDishes.FirstOrDefault(x => x.Basket.User.Id == UserId && x.DishId == "test").Quantity;

            Assert.Equal(expectedCount, actualCount);
        }

        [Fact]
        public async Task AddQuantityToDishAsyncThrowsExceptionIfThereisNoSuchDrinkId()
        {
            await Assert.ThrowsAsync<InvalidOperationException>(async () => await this.BasketService.AddQuantityToDishAsync("invalid", UserId, 30));
        }

        [Fact]
        public async Task AddQuantityToDishAsyncThrowsExceptionIfThereisNoSuchUserId()
        {
            await Assert.ThrowsAsync<InvalidOperationException>(async () => await this.BasketService.AddQuantityToDishAsync("invalid", "INVALID", 30));
        }

        [Fact]
        public async Task RemoveDishAsyncSubstractsQuantityCorrectly()
        {
            await this.AddDishesToDB();
            await this.AddBasketDishesToUserBasket();

            await this.BasketService.RemoveDishAsync("test", UserId, 1);
            var expected = 2;
            var actual = this.DbContext.BasketsDishes.FirstOrDefault(x => x.Basket.User.Id == UserId && x.DishId == "test").Quantity;

            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task RemoveDishAsyncRemovesItIfTheQuantityIsTheSameAsCurrentOne()
        {
            await this.AddDishesToDB();
            await this.AddBasketDishesToUserBasket();

            await this.BasketService.RemoveDishAsync("test", UserId, 3);
            var dish = this.DbContext.BasketsDishes.FirstOrDefault(x => x.Basket.User.Id == UserId && x.DishId == "test");

            Assert.Null(dish);
        }

        [Fact]
        public async Task RemoveDishAsyncRemovesItWhenTheQuantityIsMoreThanTheCurrent()
        {
            await this.AddDishesToDB();
            await this.AddBasketDishesToUserBasket();

            await this.BasketService.RemoveDishAsync("test", UserId, 50);
            var dish = this.DbContext.BasketsDishes.FirstOrDefault(x => x.Basket.User.Id == UserId && x.DishId == "test");

            Assert.Null(dish);
        }

        [Fact]
        public async Task RemoveDishAsyncRemoveTheDishWhenTheQuantityIs0()
        {
            await this.AddDishesToDB();
            await this.AddBasketDishesToUserBasket();

            await this.BasketService.RemoveDishAsync("test", UserId, 0);
            var dish = this.DbContext.BasketsDishes.FirstOrDefault(x => x.Basket.User.Id == UserId && x.DishId == "test");

            Assert.Null(dish);
        }

        [Fact]
        public async Task RemoveDrinkAsyncSubstractsQuantityCorrectly()
        {
            await this.AddDrinksToDB();
            await this.AddBasketDrinksToUserBasket();

            await this.BasketService.RemoveDrinkAsync("test", UserId, 1);
            var expected = 2;
            var actual = this.DbContext.BasketsDrinks.FirstOrDefault(x => x.Basket.User.Id == UserId && x.DrinkId == "test").Quantity;

            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task RemoveDrinkAsyncRemovesItIfTheQuantityIsTheSameAsCurrentOne()
        {
            await this.AddDrinksToDB();
            await this.AddBasketDrinksToUserBasket();

            await this.BasketService.RemoveDrinkAsync("test", UserId, 3);
            var drink = this.DbContext.BasketsDrinks.FirstOrDefault(x => x.Basket.User.Id == UserId && x.DrinkId == "test");

            Assert.Null(drink);
        }

        [Fact]
        public async Task RemoveDrinkAsyncRemovesItWhenTheQuantityIsMoreThanTheCurrent()
        {
            await this.AddDrinksToDB();
            await this.AddBasketDrinksToUserBasket();

            await this.BasketService.RemoveDrinkAsync("test", UserId, 50);
            var drink = this.DbContext.BasketsDrinks.FirstOrDefault(x => x.Basket.User.Id == UserId && x.DrinkId == "test");

            Assert.Null(drink);
        }

        [Fact]
        public async Task RemoveDrinkAsyncRemovesItWhenTheQuantityIs0()
        {
            await this.AddDrinksToDB();
            await this.AddBasketDrinksToUserBasket();

            await this.BasketService.RemoveDrinkAsync("test", UserId, 0);
            var drink = this.DbContext.BasketsDrinks.FirstOrDefault(x => x.Basket.User.Id == UserId && x.DrinkId == "test");

            Assert.Null(drink);
        }

        [Fact]
        public async Task GetTotalPriceWorksCorrectlyWithoutPromoCode()
        {
            await this.AddDishesToDB();
            await this.AddDrinksToDB();
            await this.AddBasketDishesToUserBasket();
            await this.AddBasketDrinksToUserBasket();

            var actual = this.BasketService.GetTotalPrice(UserId);

            var expected = 3 * 4 * 10;

            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task GetTotalPriceWorksCorrectlyWithPromoCode()
        {
            await this.AddDishesToDB();
            await this.AddDrinksToDB();
            await this.AddBasketDishesToUserBasket();
            await this.AddBasketDrinksToUserBasket();
            await this.AddPromoCodeToUser();

            var actual = this.BasketService.GetTotalPrice(UserId);

            var expected = 3 * 4 * 10 * 0.81M;

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void GetBasketWorksReturnThrowsExceptionWhenTheUserIsInvalid()
        {
            Assert.Throws<InvalidOperationException>(() => this.BasketService.GetBasket("INVALID"));
        }

        [Fact]
        public async Task GetBasketWorksCorrectly()
        {
            await this.AddDishesToDB();
            await this.AddDrinksToDB();
            await this.AddBasketDishesToUserBasket();
            await this.AddBasketDrinksToUserBasket();

            var expected = this.DbContext.Baskets.Where(x => x.User.Id == UserId)
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
                        .First();

            var actual = this.BasketService.GetBasket(UserId);

            actual.IsDeepEqual(expected);
        }

        [Fact]
        public async Task RemoveBasketItemsAsyncWorksCorrectly()
        {
            await this.AddDishesToDB();
            await this.AddDrinksToDB();
            await this.AddBasketDishesToUserBasket();
            await this.AddBasketDrinksToUserBasket();
            await this.AddPromoCodeToUser();

            await this.BasketService.RemoveBasketItemsAsync(UserId);
            var basket = this.DbContext.Baskets.FirstOrDefault(x => x.User.Id == UserId);
            var expectedCount = 0;

            Assert.Null(basket.PromoCode);
            Assert.Equal(expectedCount, basket.Drinks.Count);
            Assert.Equal(expectedCount, basket.Dishes.Count);
        }

        [Fact]
        public async Task RemoveBasketItemsAsyncThrowsExceptionWhenGivenInvalidUser()
        {
            await this.AddDishesToDB();
            await this.AddDrinksToDB();
            await this.AddBasketDishesToUserBasket();
            await this.AddBasketDrinksToUserBasket();
            await this.AddPromoCodeToUser();

            await Assert.ThrowsAsync<NullReferenceException>(async() => await this.BasketService.RemoveBasketItemsAsync("INVALID"));
        }

        [Fact]
        public async Task AddPromoCodeAsyncWorksCorrectly()
        {
            await this.AddDishesToDB();
            await this.AddBasketDishesToUserBasket();
            this.DbContext.PromoCodes.Add(new PromoCode()
            {
                Id = 1,
                Code = "ASDFGH",
                PromoPercent = 20,
                ExpirationDate = DateTime.UtcNow.AddDays(5),
                MaxUsageTimes = 100,
                UsedTimes = 10,
            });

            await this.DbContext.SaveChangesAsync();

            await this.BasketService.AddPromoCodeAsync("ASDFGH", UserId);
            var expected = this.DbContext.PromoCodes.FirstOrDefault(x => x.Id == 1);
            var actual = this.DbContext.Baskets.FirstOrDefault(x => x.User.Id == UserId).PromoCode;

            actual.IsDeepEqual(expected);
        }

        [Fact]
        public async Task AddPromoCodeThrowsExceptionWhenGivenExpiredCode()
        {
            await this.AddDishesToDB();
            await this.AddBasketDishesToUserBasket();
            this.DbContext.PromoCodes.Add(new PromoCode()
            {
                Id = 1,
                Code = "ASDFGH",
                PromoPercent = 20,
                ExpirationDate = DateTime.UtcNow.AddHours(-1),
                MaxUsageTimes = 100,
                UsedTimes = 10,
            });
            await this.DbContext.SaveChangesAsync();

            await Assert.ThrowsAsync<Exception>(async () => await this.BasketService.AddPromoCodeAsync("ASDFGH", UserId));
        }

        [Fact]
        public async Task AddPromoCodeThrowsExceptionWhenGivenOverusedCode()
        {
            await this.AddDishesToDB();
            await this.AddBasketDishesToUserBasket();
            this.DbContext.PromoCodes.Add(new PromoCode()
            {
                Id = 1,
                Code = "ASDFGH",
                PromoPercent = 20,
                ExpirationDate = DateTime.UtcNow.AddHours(-1),
                MaxUsageTimes = 100,
                UsedTimes = 100,
            });
            await this.DbContext.SaveChangesAsync();

            await Assert.ThrowsAsync<Exception>(async () => await this.BasketService.AddPromoCodeAsync("ASDFGH", UserId));
        }

        [Fact]
        public async Task AddPromoCodeThrowsExceptionWhenGivenInvalidCode()
        {
            await this.AddDishesToDB();
            await this.AddBasketDishesToUserBasket();
            this.DbContext.PromoCodes.Add(new PromoCode()
            {
                Id = 1,
                Code = "ASDFGH",
                PromoPercent = 20,
                ExpirationDate = DateTime.UtcNow.AddHours(1),
                MaxUsageTimes = 100,
                UsedTimes = 100,
            });
            await this.DbContext.SaveChangesAsync();

            await Assert.ThrowsAsync<InvalidOperationException>(async () => await this.BasketService.AddPromoCodeAsync("ASDJASVFU", UserId));
        }

        [Fact]
        public async Task AddPromoCodeThrowsExceptionWhenGivenInvalidUserId()
        {
            await this.AddDishesToDB();
            await this.AddBasketDishesToUserBasket();
            this.DbContext.PromoCodes.Add(new PromoCode()
            {
                Id = 1,
                Code = "ASDFGH",
                PromoPercent = 20,
                ExpirationDate = DateTime.UtcNow.AddHours(1),
                MaxUsageTimes = 100,
                UsedTimes = 10,
            });
            await this.DbContext.SaveChangesAsync();

            await Assert.ThrowsAsync<NullReferenceException>(async () => await this.BasketService.AddPromoCodeAsync("ASDFGH", "INVALID!"));
        }

        [Fact]
        public async Task GetBasketPromoCodeByIdWorksCorrectly()
        {
            await this.AddDishesToDB();
            await this.AddBasketDishesToUserBasket();
            await this.AddPromoCodeToUser();

            var expected = this.DbContext.PromoCodes.Select(x => new BasketPromoCodeViewModel()
            {
                Code = x.Code,
                ExpirationDate = x.ExpirationDate.ToLocalTime().ToString("dd/MM/yyyy HH:mm:ss"),
            }).FirstOrDefault();

            var actual = this.BasketService.GetBasketPromoCodeById(UserId);

            actual.IsDeepEqual(expected);
        }

        [Fact]
        public async Task GetBasketPromoCodeByIdReturnsNullWhenGivenInvalidUserId()
        {
            await this.AddDishesToDB();
            await this.AddBasketDishesToUserBasket();
            await this.AddPromoCodeToUser();

            var actual = this.BasketService.GetBasketPromoCodeById("INVALID");

            Assert.Null(actual);
        }

        [Fact]
        public async Task RemovePromoCodeByIdAsyncWorksCorrectly()
        {
            await this.AddDishesToDB();
            await this.AddBasketDishesToUserBasket();
            await this.AddPromoCodeToUser();

            await this.BasketService.RemovePromoCodeByIdAsync(UserId);

            var result = this.DbContext.Baskets.FirstOrDefault().PromoCode;
            Assert.Null(result);
        }

        [Fact]
        public async Task RemovePromoCodeByIdAsyncThrowsExceptionWhenGivenInvalidId()
        {
            await this.AddDishesToDB();
            await this.AddBasketDishesToUserBasket();
            await this.AddPromoCodeToUser();

            await Assert.ThrowsAsync<NullReferenceException>(async() => await this.BasketService.RemovePromoCodeByIdAsync("INVALID"));
        }

        private async Task AddBasketDrinksToUserBasket()
        {
            await this.BasketService.AddToBasketAsync(
                new AddItemToBasketViewModel()
                {
                    Type = "Drink",
                    Count = 3,
                    Id = "test",
                },
                UserId);

            await this.BasketService.AddToBasketAsync(
                new AddItemToBasketViewModel()
                {
                    Type = "Drink",
                    Count = 3,
                    Id = "test1",
                },
                UserId);
        }

        private async Task AddBasketDishesToUserBasket()
        {
            await this.BasketService.AddToBasketAsync(
                new AddItemToBasketViewModel()
                {
                    Type = "Dish",
                    Count = 3,
                    Id = "test",
                },
                UserId);

            await this.BasketService.AddToBasketAsync(
                new AddItemToBasketViewModel()
                {
                    Type = "Dish",
                    Count = 3,
                    Id = "test1",
                },
                UserId);
        }

        private async Task AddDrinksToDB()
        {
            this.DbContext.Drinks.Add(new Drink()
            {
                Id = "test",
                Price = 10,
                Name = "test",
                AdditionalInfo = "test",
                Weight = 50,
                PackagingType = new PackagingType() { Name = "test" },
                AlchoholByVolume = 0,
                DrinkType = new DrinkType()
                {
                    Name = "test",
                },
            });

            this.DbContext.Drinks.Add(new Drink()
            {
                Id = "test1",
                Price = 10,
                Name = "test1",
                AdditionalInfo = "test1",
                Weight = 50,
                PackagingType = new PackagingType() { Name = "test1" },
                AlchoholByVolume = 0,
                DrinkType = new DrinkType()
                {
                    Name = "test1",
                },
            });

            if (!this.DbContext.Users.Any())
            {
                this.DbContext.Users.Add(new ApplicationUser()
                {
                    Id = UserId,
                });
            }

            await this.DbContext.SaveChangesAsync();
        }

        private async Task AddDishesToDB()
        {
            this.DbContext.Dishes.Add(new Dish()
            {
                Id = "test",
                Price = 10,
                Name = "test1",
                AdditionalInfo = "test1",
                Weight = 50,
                DishType = new DishType()
                {
                    Name = "test1",
                },
            });

            this.DbContext.Dishes.Add(new Dish()
            {
                Id = "test1",
                Price = 10,
                Name = "test1",
                AdditionalInfo = "test1",
                Weight = 50,
                DishType = new DishType()
                {
                    Name = "test1",
                },
            });

            if (!this.DbContext.Users.Any())
            {
                this.DbContext.Users.Add(new ApplicationUser()
                {
                    Id = UserId,
                });
            }

            await this.DbContext.SaveChangesAsync();
        }

        private async Task AddPromoCodeToUser()
        {
            var dishTypes = this.DbContext.DishTypes.ToList();
            var drinkTypes = this.DbContext.DrinkTypes.ToList();

            var promoCode = new PromoCode()
            {
                Id = 1,
                ValidDishCategories = dishTypes,
                ValidDrinkCategories = drinkTypes,
                PromoPercent = 19,
            };

            await this.DbContext.PromoCodes.AddAsync(promoCode);
            await this.DbContext.SaveChangesAsync();

            this.DbContext.Baskets.FirstOrDefault(x => x.User.Id == UserId).PromoCode = promoCode;
        }
    }
}
