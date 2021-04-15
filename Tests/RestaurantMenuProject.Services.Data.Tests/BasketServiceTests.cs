namespace RestaurantMenuProject.Services.Data.Tests
{
    using DeepEqual.Syntax;
    using Microsoft.Extensions.DependencyInjection;
    using RestaurantMenuProject.Data.Models;
    using RestaurantMenuProject.Data.Models.Enums;
    using RestaurantMenuProject.Services.Data.Contracts;
    using RestaurantMenuProject.Web.ViewModels;
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
                Price = 50,
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

            this.DbContext.Users.Add(new ApplicationUser()
            {
                Id = UserId,
            });

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

            this.DbContext.Users.Add(new ApplicationUser()
            {
                Id = UserId,
            });

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
