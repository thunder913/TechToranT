namespace RestaurantMenuProject.Services.Data.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Dynamic.Core;
    using System.Threading.Tasks;

    using DeepEqual.Syntax;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.DependencyInjection;
    using RestaurantMenuProject.Data.Models;
    using RestaurantMenuProject.Data.Models.Enums;
    using RestaurantMenuProject.Services.Data.Contracts;
    using RestaurantMenuProject.Services.Mapping;
    using RestaurantMenuProject.Web.ViewModels;
    using Xunit;

    public class OrderServiceTests : BaseServiceTests
    {
        private IOrderService OrderService => this.ServiceProvider.GetRequiredService<IOrderService>();

        [Theory]
        [InlineData(10, 1)]
        [InlineData(2, 2)]
        [InlineData(1, 3)]
        [InlineData(100, 1)]
        [InlineData(50, 1)]
        public async Task GetOrderViewModelsByUserIdWorksCorrectly(int itemsPerPage, int page, string userId = null)
        {
            await this.PopulateDB();

            var expected = this.DbContext.Orders
                    .Where(x => userId == null || x.ClientId == userId)
                    .Include(x => x.Client)
                    .OrderByDescending(x => x.CreatedOn)
                    .Skip((page - 1) * itemsPerPage)
                    .Take(itemsPerPage)
                    .To<OrderInListViewModel>()
                    .ToList();
            var actual = this.OrderService.GetOrderViewModelsByUserId(page, itemsPerPage, userId);

            actual.IsDeepEqual(expected);
        }

        [Fact]
        public async Task GetUserOrdersCountWorksCorrectly()
        {
            await this.PopulateDB();
            var userId = "user3";

            var expectedCount = this.DbContext.Orders.Where(x => x.Client.Id == userId).Count();
            var actualCount = this.OrderService.GetUserOrdersCount(userId);

            Assert.Equal(expectedCount, actualCount);
        }

        [Fact]
        public async Task MakeOrderAsyncWorksCorrectlyWithoutPromoCode()
        {
            await this.PopulateDB();
            var userId = "user3";
            var tableCode = "code1";
            var user = this.DbContext.Users.FirstOrDefault(x => x.Id == userId);
            var expectedOrdersCount = user.Orders.Count + 1;

            var basket = new Basket()
            {
                Id = "user3",
                User = user,
                Dishes = new List<BasketDish>() { new BasketDish() { BasketId = "user3", DishId = "test1", Quantity = 2 }, new BasketDish() { BasketId = "user3", DishId = "test2", Quantity = 5 } },
                Drinks = new List<BasketDrink>() { new BasketDrink() { BasketId = "user3", DrinkId = "test1", Quantity = 2 }, new BasketDrink() { BasketId = "user3", DrinkId = "test2", Quantity = 5 } },
            };
            this.DbContext.Baskets.Add(basket);
            await this.DbContext.SaveChangesAsync();

            var orderId = await this.OrderService.MakeOrderAsync(userId, tableCode);
            var actualBasket = this.DbContext.Baskets.FirstOrDefault(x => x.Id == userId);
            var order = this.DbContext.Orders.FirstOrDefault(x => x.Id == orderId);

            Assert.Equal(expectedOrdersCount, this.DbContext.Orders.Where(x => x.ClientId == userId).Count());
            Assert.Equal(0, actualBasket.Dishes.Count);
            Assert.Equal(0, actualBasket.Drinks.Count);
            Assert.Equal(2, order.OrderDishes.Count);
            Assert.Equal(2, order.OrderDrinks.Count);
        }

        [Fact]
        public async Task MakeOrderAsyncWorksCorrectlyWithPromoCode()
        {
            await this.PopulateDB();
            var userId = "user3";
            var tableCode = "code1";
            var user = this.DbContext.Users.FirstOrDefault(x => x.Id == userId);
            var expectedOrdersCount = user.Orders.Count + 1;
            var promoCode = this.DbContext.PromoCodes.FirstOrDefault(x => x.Id == 1);

            var basket = new Basket()
            {
                Id = "user3",
                User = user,
                Dishes = new List<BasketDish>() { new BasketDish() { BasketId = "user3", DishId = "test1", Quantity = 2 }, new BasketDish() { BasketId = "user3", DishId = "test2", Quantity = 5 } },
                Drinks = new List<BasketDrink>() { new BasketDrink() { BasketId = "user3", DrinkId = "test1", Quantity = 2 }, new BasketDrink() { BasketId = "user3", DrinkId = "test2", Quantity = 5 } },
                PromoCode = promoCode,
            };
            this.DbContext.Baskets.Add(basket);
            await this.DbContext.SaveChangesAsync();

            var orderId = await this.OrderService.MakeOrderAsync(userId, tableCode);
            var actualBasket = this.DbContext.Baskets.FirstOrDefault(x => x.Id == userId);
            var order = this.DbContext.Orders.FirstOrDefault(x => x.Id == orderId);

            Assert.Equal(expectedOrdersCount, this.DbContext.Orders.Where(x => x.ClientId == userId).Count());
            Assert.Equal(0, actualBasket.Dishes.Count);
            Assert.Equal(0, actualBasket.Drinks.Count);
            Assert.Equal(2, order.OrderDishes.Count);
            Assert.Equal(2, order.OrderDrinks.Count);
            Assert.NotNull(order.PromoCode);
            Assert.Null(basket.PromoCode);
        }

        [Fact]
        public async Task MakeOrderAsyncThrowsExceptionWhenTableIsNonExistant()
        {
            await Assert.ThrowsAsync<InvalidOperationException>(async () => await this.OrderService.MakeOrderAsync("user3", "INVALID"));
        }

        [Fact]
        public async Task MakeOrderASyncThrowsExceptionWhenGivenEmptyBasket()
        {
            await this.PopulateDB();
            var userId = "user3";
            var tableCode = "code1";
            var user = this.DbContext.Users.FirstOrDefault(x => x.Id == userId);
            var expectedOrdersCount = user.Orders.Count + 1;
            var promoCode = this.DbContext.PromoCodes.FirstOrDefault(x => x.Id == 1);

            var basket = new Basket()
            {
                Id = "user3",
                User = user,
                PromoCode = promoCode,
            };
            this.DbContext.Baskets.Add(basket);
            await this.DbContext.SaveChangesAsync();

            await Assert.ThrowsAsync<Exception>(async () => await this.OrderService.MakeOrderAsync("user3", tableCode));

            basket.Drinks = new List<BasketDrink>() { new BasketDrink() { BasketId = "user3", DrinkId = "test1", Quantity = 2 }, new BasketDrink() { BasketId = "user3", DrinkId = "test2", Quantity = 5 } };

            await Assert.ThrowsAsync<Exception>(async () => await this.OrderService.MakeOrderAsync("user3", tableCode));
        }

        [Fact]
        public async Task CanceOrderAsyncWorksCorrectlyWhenGivenPendingOrder()
        {
            await this.PopulateDB();
            var order = this.DbContext.Orders.FirstOrDefault();

            var actual = await this.OrderService.CancelOrderAsync(order.Id);

            Assert.Equal(ProcessType.Cancelled, order.ProcessType);
            Assert.True(actual);
        }

        [Fact]
        public async Task CancelOrderAsyncWorksCorrectlyWhenGivenNonPendingOrder()
        {
            await this.PopulateDB();
            var order = this.DbContext.Orders.FirstOrDefault();
            order.ProcessType = ProcessType.Cooking;

            var actual = await this.OrderService.CancelOrderAsync(order.Id);

            Assert.False(actual);
        }

        [Fact]
        public async Task GetAllFoodItemsByIdWorksCorrectly()
        {
            await this.PopulateDB();

            var orderId = "order2";

            var orderDrinks = this.DbContext.OrderDrinks
                    .Where(x => x.OrderId == orderId)
                    .To<FoodItemViewModel>()
                    .ToList();
            var orderDishes = this.DbContext.OrdersDishes
                    .Where(x => x.OrderId == orderId)
                    .To<FoodItemViewModel>()
                    .ToList();

            var expected = orderDishes;
            expected.AddRange(orderDrinks);
            var actual = this.OrderService.GetAllFoodItemsById(orderId);

            actual.IsDeepEqual(expected);
        }

        [Theory]
        [InlineData("", "", "")]
        [InlineData("price", "desc", "")]
        [InlineData("price", "desc", "a")]
        [InlineData("price", "desc", "/")]
        [InlineData("price", "desc", ":")]
        [InlineData("price", "desc", "@")]
        [InlineData("price", "desc", "p")]
        [InlineData("price", "desc", " ")]
        [InlineData("price", "desc", "I")]
        [InlineData("price", "desc", ".")]
        [InlineData("price", "desc", "0")]
        [InlineData("price", "desc", "1")]
        [InlineData("price", "desc", "5")]
        [InlineData("price", "desc", "202")]
        [InlineData("price", "desc", "Pending")]
        [InlineData("price", "desc", "Cooking")]
        public async Task GetAllOrdersWorksCorrectly(string sortColumn, string sortDirection, string searchValue) 
        {
            await this.PopulateDB();
            var orders = this.DbContext.Orders
                .To<OrderInListViewModel>();

            if (!(string.IsNullOrWhiteSpace(sortColumn) || string.IsNullOrWhiteSpace(sortDirection)))
            {
                orders = orders.OrderBy(sortColumn + " " + sortDirection);
            }

            var expected = orders.ToList();

            if (!string.IsNullOrWhiteSpace(searchValue))
            {
                expected = orders.Where(m =>
                      m.Price.ToString().Contains(searchValue)
                      || m.Email.ToLower().Contains(searchValue.ToLower())
                      || m.Status.ToString().ToLower().Contains(searchValue.ToLower())
                      || m.Date.ToLocalTime().ToString("dd/MM/yyyy, HH:mm:ss").Contains(searchValue)
                      || m.FullName.ToLower().Contains(searchValue))
                      .ToList();
            }
            var actual = this.OrderService.GetAllOrders(sortColumn, sortDirection, searchValue);

            actual.IsDeepEqual(expected);

        }

        [Fact]
        public async Task GetOrdersWithStatusWorksCorrectly()
        {
            await this.PopulateDB();
            var processType = ProcessType.Pending;

            var expected = this.DbContext.Orders
                .Where(x => x.ProcessType == processType)
                .OrderBy(x => x.CreatedOn)
                .To<OrderInListViewModel>()
                .ToList();
            var actual = this.OrderService.GetOrdersWithStatus(processType);

            actual.IsDeepEqual(expected);
        }

        [Fact]
        public async Task ChangeOrderStatusAsyncWorksCorrectly()
        {
            await this.PopulateDB();

            var oldProcessType = ProcessType.Pending;
            var newProcessType = ProcessType.InProcess;

            var order = this.DbContext.Orders.FirstOrDefault();
            await this.OrderService.ChangeOrderStatusAsync(oldProcessType, newProcessType, order.Id);

            Assert.Equal(order.ProcessType, newProcessType);
        }

        [Fact]
        public async Task ChangeOrderStatusAsyncThrowsWhenGivenNotTheRightProcessType()
        {
            await this.PopulateDB();

            var oldProcessType = ProcessType.Cooking;
            var newProcessType = ProcessType.Cooked;

            await Assert.ThrowsAsync<InvalidOperationException>(async () => await this.OrderService.ChangeOrderStatusAsync(oldProcessType, newProcessType, "order1"));
        }

        [Fact]
        public async Task ChangeOrderStatusAsyncThrowsWhenGivenTheSameProcessType()
        {
            await this.PopulateDB();

            var oldProcessType = ProcessType.Pending;
            var newProcessType = ProcessType.Pending;

            await Assert.ThrowsAsync<InvalidOperationException>(async () => await this.OrderService.ChangeOrderStatusAsync(oldProcessType, newProcessType, "order1"));
        }

        [Fact]
        public async Task ChangeOrderStatusAsyncThrowsWhenGivenInvalidOrderId()
        {
            await this.PopulateDB();

            var oldProcessType = ProcessType.Pending;
            var newProcessType = ProcessType.InProcess;

            await Assert.ThrowsAsync<InvalidOperationException>(async () => await this.OrderService.ChangeOrderStatusAsync(oldProcessType, newProcessType, "InVALID!"));
        }

        [Fact]
        public async Task AddWaiterToOrderAsyncWorksCorrectly()
        {
            await this.PopulateDB();

            var order = this.DbContext.Orders.FirstOrDefault();
            var waiterId = "user1";

            await this.OrderService.AddWaiterToOrderAsync(order.Id, waiterId);

            Assert.Equal(order.WaiterId, waiterId);
        }

        [Fact]
        public async Task AddWaiterToOrderAsyncThrowsWhenGivenInvalidOrder()
        {
            await Assert.ThrowsAsync<InvalidOperationException>(async () => await this.OrderService.AddWaiterToOrderAsync("INVALID!", "user1"));
        }

        [Fact]
        public async Task GetActiveOrdersWorksCorrectly()
        {
            await this.PopulateDB();

            var waiterId = "user2";
            this.DbContext.Orders.FirstOrDefault(x => x.Id == "order2").ProcessType = ProcessType.Cooking;
            this.DbContext.Orders.FirstOrDefault(x => x.Id == "order3").ProcessType = ProcessType.Completed;
            await this.DbContext.SaveChangesAsync();

            var actual = this.OrderService.GetActiveOrders(waiterId);
            var expected = this.DbContext.Orders
                .Where(x => x.WaiterId == waiterId && x.ProcessType != ProcessType.Completed && x.ProcessType != ProcessType.Pending)
                .OrderBy(x => x.CreatedOn)
                .To<ActiveOrderViewModel>()
                .ToList();

            actual.IsDeepEqual(expected);
            Assert.Equal(38.1, actual.FirstOrDefault().ReadyPercent);
        }

        [Fact]
        public async Task GetWaiterViewModelWorksCorrectly()
        {
            await this.PopulateDB();
            var order = this.DbContext.Orders.FirstOrDefault();
            order.ProcessType = ProcessType.Cooking;
            await this.DbContext.SaveChangesAsync();

            var expected = new WaiterViewModel();
            var waiterId = "user2";
            expected.NewOrders = this.OrderService.GetOrdersWithStatus(ProcessType.Pending);
            expected.ActiveOrders = this.OrderService.GetActiveOrders(waiterId);
            var actual = this.OrderService.GetWaiterViewModel(waiterId);

            actual.IsDeepEqual(expected);
        }

        [Fact]
        public async Task FinishOrderAsyncWorksCorrectly()
        {
            await this.PopulateDB();
            var orderId = "order2";
            var order = this.DbContext.Orders.FirstOrDefault(x => x.Id == orderId);
            order.ProcessType = ProcessType.Delivered;
            order.PaidOn = DateTime.UtcNow;
            await this.DbContext.SaveChangesAsync();

            await this.OrderService.FinishOrderAsync(orderId);

            Assert.Equal(ProcessType.Completed, order.ProcessType);
        }

        [Fact]
        public async Task FinishOrderAsynsThrowsWhenNotPaid()
        {
            await this.PopulateDB();
            var orderId = "order2";
            var order = this.DbContext.Orders.FirstOrDefault(x => x.Id == orderId);
            order.ProcessType = ProcessType.Delivered;
            await this.DbContext.SaveChangesAsync();

            await Assert.ThrowsAsync<InvalidOperationException>(async () => await this.OrderService.FinishOrderAsync(orderId));
        }

        [Fact]
        public async Task FinishOrderAsynsThrowsWhenNotDelivered()
        {
            await this.PopulateDB();
            var orderId = "order2";
            var order = this.DbContext.Orders.FirstOrDefault(x => x.Id == orderId);
            order.ProcessType = ProcessType.Cooking;
            await this.DbContext.SaveChangesAsync();
            order.PaidOn = DateTime.UtcNow;
            await Assert.ThrowsAsync<InvalidOperationException>(async () => await this.OrderService.FinishOrderAsync(orderId));
        }

        [Fact]
        public async Task GetChefViewModelAsync()
        {
            await this.PopulateDB();

            var expected = new ChefViewModel();
            expected.NewOrders = this.OrderService.GetOrdersWithStatus(ProcessType.InProcess);
            expected.FoodTypes = this.OrderService.GetCookFoodTypes(null);
            var actual = this.OrderService.GetChefViewModel();

            actual.IsDeepEqual(expected);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("order2")]
        public async Task GetCookFoodTypesWorksCorrectly(string orderId)
        {
            await this.PopulateDB();
            var orders = this.DbContext.Orders.ToList();
            foreach (var order in orders)
            {
                order.ProcessType = ProcessType.Cooking;
            }
            await this.DbContext.SaveChangesAsync();

            var allDrinks = this.DbContext.OrderDrinks
                .Where(x => x.Order.ProcessType == ProcessType.Cooking && x.Count - x.DeliveredCount > 0 && (orderId == null || x.OrderId == orderId))
                .OrderBy(x => x.Order.CreatedOn)
                .Select(x => new CookItemViewModel()
                {
                    Count = x.Count - x.DeliveredCount,
                    FoodId = x.DrinkId,
                    FoodName = x.Drink.Name,
                    OrderId = x.OrderId,
                }).ToList();
            var drinks = new CookFoodCategoriesViewModel()
            {
                FoodType = FoodType.Drink,
                CategoryName = "Drinks",
                ItemsToCook = allDrinks,
            };

            var dishTypes = this.DbContext.OrdersDishes
                .GroupBy(x => x.Dish.DishTypeId)
                .Select(x => x.Key);
            var dishes = new HashSet<CookFoodCategoriesViewModel>();

            foreach (var type in dishTypes)
            {
                var typeDishes = this.DbContext.OrdersDishes.Where(x => (x.Dish.DishTypeId == type && x.Order.ProcessType == ProcessType.Cooking && x.Count - x.DeliveredCount > 0) && (orderId == null || x.OrderId == orderId))
                    .OrderBy(x => x.Order.CreatedOn)
                    .Select(x => new CookItemViewModel()
                    {
                        OrderId = x.OrderId,
                        FoodId = x.Dish.Id,
                        Count = x.Count - x.DeliveredCount,
                        FoodName = x.Dish.Name,
                    }).ToList();
                dishes.Add(new CookFoodCategoriesViewModel()
                {
                    CategoryName = this.DbContext.DishTypes.FirstOrDefault(x => x.Id == type).Name,
                    FoodType = FoodType.Dish,
                    ItemsToCook = typeDishes,
                });
            }

            var expected = new HashSet<CookFoodCategoriesViewModel>();
            expected.Add(drinks);
            foreach (var dish in dishes)
            {
                expected.Add(dish);
            }
            var actual = this.OrderService.GetCookFoodTypes(null);


            actual.IsDeepEqual(expected);
        }

        [Fact]
        public async Task AddDeliveredCountToOrderDishAsyncWorksCorrectly()
        {
            await this.PopulateDB();
            var foodId = "test1";
            var orderId = "order2";
            var toAdd = 2;
            var expectedCount = this.DbContext.OrdersDishes.FirstOrDefault(x => x.OrderId == orderId && x.DishId == foodId).DeliveredCount + toAdd;

            var cookViewModel = new CookFinishItemViewModel()
            {
                FoodId = foodId,
                OrderId = orderId,
            };
            await this.OrderService.AddDeliveredCountToOrderDishAsync(toAdd, cookViewModel);
            var actualCount = this.DbContext.OrdersDishes.FirstOrDefault(x => x.OrderId == orderId && x.DishId == foodId).DeliveredCount;

            Assert.Equal(expectedCount, actualCount);
        }

        [Fact]
        public async Task AddDeliveredCountToOrderDrinkAsyncWorksCorrectly()
        {
            await this.PopulateDB();
            var foodId = "test2";
            var orderId = "order2";
            var toAdd = 2;
            var expectedCount = this.DbContext.OrderDrinks.FirstOrDefault(x => x.OrderId == orderId && x.DrinkId == foodId).DeliveredCount + toAdd;

            var cookViewModel = new CookFinishItemViewModel()
            {
                FoodId = foodId,
                OrderId = orderId,
            };
            await this.OrderService.AddDeliveredCountToOrderDrinkAsync(toAdd, cookViewModel);
            var actualCount = this.DbContext.OrderDrinks.FirstOrDefault(x => x.OrderId == orderId && x.DrinkId == foodId).DeliveredCount;

            Assert.Equal(expectedCount, actualCount);
        }

        [Fact]
        public async Task AddDeliveredCountToOrderDishAsyncThrowsWhenItemHasAlreadyBeenMade()
        {
            await this.PopulateDB();

            var orderDish = this.DbContext.OrdersDishes.FirstOrDefault();
            orderDish.DeliveredCount = orderDish.Count;
            var cookViewModel = new CookFinishItemViewModel()
            {
                FoodId = orderDish.Dish.Id,
                OrderId = orderDish.OrderId,
            };

            await Assert.ThrowsAsync<InvalidOperationException>(async () => await this.OrderService.AddDeliveredCountToOrderDishAsync(1, cookViewModel));
        }

        [Fact]
        public async Task AddDeliveredCountToOrderDrinkAsyncThrowsWhenItemHasAlreadyBeenMade()
        {
            await this.PopulateDB();

            var orderDish = this.DbContext.OrderDrinks.FirstOrDefault();
            orderDish.DeliveredCount = orderDish.Count;
            var cookViewModel = new CookFinishItemViewModel()
            {
                FoodId = orderDish.DrinkId,
                OrderId = orderDish.OrderId,
            };

            await Assert.ThrowsAsync<InvalidOperationException>(async () => await this.OrderService.AddDeliveredCountToOrderDrinkAsync(1, cookViewModel));
        }

        private async Task PopulateDB()
        {
            this.DbContext.Ingredients.Add(new Ingredient()
            {
                Name = "test1",
                Id = 1,
            });

            this.DbContext.Ingredients.Add(new Ingredient()
            {
                Name = "test2",
                Id = 2,
            });

            this.DbContext.DishTypes.Add(new DishType()
            {
                Id = 1,
                Name = "test",
            });

            this.DbContext.DishTypes.Add(new DishType()
            {
                Id = 2,
                Name = "test2",
            });

            this.DbContext.DrinkTypes.Add(new DrinkType()
            {
                Id = 1,
                Name = "test1",
            });

            this.DbContext.DrinkTypes.Add(new DrinkType()
            {
                Id = 2,
                Name = "test2",
            });

            this.DbContext.PackagingTypes.Add(new PackagingType()
            {
                Id = 1,
                Name = "test1",
            });

            this.DbContext.PackagingTypes.Add(new PackagingType()
            {
                Id = 2,
                Name = "test2",
            });

            await this.DbContext.SaveChangesAsync();

            var firstIngredients = this.DbContext.Ingredients.Where(x => x.Id == 1 || x.Id == 2).ToList();
            var secondIngredients = this.DbContext.Ingredients.Where(x => x.Id == 1).ToList();

            await this.DbContext.Dishes.AddAsync(
                new Dish()
                {
                    Id = "test1",
                    Name = "test1",
                    AdditionalInfo = "test1",
                    Weight = 50,
                    Price = 10,
                    Image = new Image() { Id = "test1", Extension = ImageExtension.jpeg },
                    DishTypeId = 1,
                    Ingredients = firstIngredients,
                    PrepareTime = 20,
                });

            await this.DbContext.Dishes.AddAsync(
                new Dish()
                {
                    Id = "test2",
                    Name = "test2",
                    AdditionalInfo = "test2",
                    Weight = 50,
                    Price = 10,
                    Image = new Image() { Id = "test2", Extension = ImageExtension.jpeg },
                    DishTypeId = 2,
                    Ingredients = firstIngredients,
                    PrepareTime = 20,
                });

            await this.DbContext.Drinks.AddAsync(
                new Drink()
                {
                    Id = "test1",
                    Name = "test1",
                    AdditionalInfo = "test1",
                    Weight = 50,
                    Price = 10,
                    Image = new Image() { Id = "test3", Extension = ImageExtension.jpeg },
                    DrinkTypeId = 1,
                    Ingredients = firstIngredients,
                    AlchoholByVolume = 20,
                    PackagingTypeId = 1,
                });

            await this.DbContext.Drinks.AddAsync(
                new Drink()
                {
                    Id = "test2",
                    Name = "test2",
                    AdditionalInfo = "test2",
                    Weight = 50,
                    Price = 10,
                    Image = new Image() { Id = "test4", Extension = ImageExtension.jpeg },
                    DrinkTypeId = 1,
                    Ingredients = firstIngredients,
                    AlchoholByVolume = 20,
                    PackagingTypeId = 2,
                });


            var role1 = new ApplicationRole()
            {
                Id = "role1",
                Name = "role1",
            };
            var role2 = new ApplicationRole()
            {
                Id = "role2",
                Name = "role2",
            };
            var role3 = new ApplicationRole()
            {
                Id = "role3",
                Name = "role3",
            };

            await this.DbContext.Roles.AddAsync(role1);
            await this.DbContext.Roles.AddAsync(role2);
            await this.DbContext.Roles.AddAsync(role3);
            await this.DbContext.SaveChangesAsync();

            await this.DbContext.Users.AddAsync(new ApplicationUser()
            {
                Id = "user1",
                FirstName = "first1",
                LastName = "last1",
                Email = "first@aaa.bg",
                PhoneNumber = "111111",
                Roles = new List<IdentityUserRole<string>>() { new IdentityUserRole<string>() { UserId = "user1", RoleId = "role1", } },
            });
            await this.DbContext.Users.AddAsync(new ApplicationUser()
            {
                Id = "user2",
                FirstName = "first2",
                LastName = "last2",
                Email = "second@aaa.bg",
                PhoneNumber = "222222",
                Roles = new List<IdentityUserRole<string>>() { new IdentityUserRole<string>() { UserId = "user2", RoleId = "role2", }, new IdentityUserRole<string>() { UserId = "user2", RoleId = "role3", } },
            });
            await this.DbContext.Users.AddAsync(new ApplicationUser()
            {
                Id = "user3",
                FirstName = "first3",
                LastName = "last3",
                Email = "third@aaa.bg",
                PhoneNumber = "333333",
                Roles = new List<IdentityUserRole<string>>() { new IdentityUserRole<string>() { UserId = "user3", RoleId = "role3", } },
            });

            await this.DbContext.Tables.AddAsync(new Table()
            {
                Id = 1,
                Number = 1,
                Code = "code1",
                Capacity = 4,
            });
            await this.DbContext.Tables.AddAsync(new Table()
            {
                Id = 2,
                Number = 2,
                Code = "code2",
                Capacity = 6,
            });
            await this.DbContext.Tables.AddAsync(new Table()
            {
                Id = 3,
                Number = 3,
                Code = "code3",
                Capacity = 2,
            });

            await this.DbContext.SaveChangesAsync();

            await this.DbContext.Orders.AddAsync(new Order()
            {
                ClientId = "user3",
                Id = "order1",
                ProcessType = ProcessType.Pending,
                TableId = 1,
                WaiterId = "user2",
            });

            await this.DbContext.Orders.AddAsync(new Order()
            {
                ClientId = "user1",
                Id = "order2",
                ProcessType = ProcessType.Pending,
                TableId = 2,
                WaiterId = "user2",
            });

            await this.DbContext.Orders.AddAsync(new Order()
            {
                ClientId = "user3",
                Id = "order3",
                ProcessType = ProcessType.Pending,
                TableId = 2,
                WaiterId = "user2",
            });

            await this.DbContext.OrdersDishes.AddAsync(new OrderDish()
            {
                OrderId = "order1",
                Count = 3,
                DeliveredCount = 1,
                DishId = "test2",
                PriceForOne = 10,
            });

            await this.DbContext.OrdersDishes.AddAsync(new OrderDish()
            {
                OrderId = "order2",
                Count = 3,
                DeliveredCount = 1,
                DishId = "test2",
                PriceForOne = 10,
            });

            await this.DbContext.OrdersDishes.AddAsync(new OrderDish()
            {
                OrderId = "order2",
                Count = 5,
                DeliveredCount = 3,
                DishId = "test1",
                PriceForOne = 10,
            });

            await this.DbContext.OrderDrinks.AddAsync(new OrderDrink()
            {
                OrderId = "order1",
                Count = 3,
                DeliveredCount = 1,
                DrinkId = "test2",
                PriceForOne = 10,
            });

            await this.DbContext.OrderDrinks.AddAsync(new OrderDrink()
            {
                OrderId = "order2",
                Count = 8,
                DeliveredCount = 1,
                DrinkId = "test2",
                PriceForOne = 10,
            });

            await this.DbContext.OrderDrinks.AddAsync(new OrderDrink()
            {
                OrderId = "order2",
                Count = 5,
                DeliveredCount = 3,
                DrinkId = "test1",
                PriceForOne = 10,
            });

            var dishTypes = this.DbContext.DishTypes.ToList();
            var drinkTypes = this.DbContext.DrinkTypes.ToList();

            await this.DbContext.PromoCodes.AddAsync(new PromoCode()
            {
                Id = 1,
                ValidDishCategories = dishTypes,
                ValidDrinkCategories = drinkTypes,
                Code = "code1",
                PromoPercent = 20,
                ExpirationDate = DateTime.Now.AddDays(1),
                MaxUsageTimes = 20,
                UsedTimes = 10,
            });

            await this.DbContext.PromoCodes.AddAsync(new PromoCode()
            {
                Id = 2,
                ValidDishCategories = dishTypes,
                ValidDrinkCategories = drinkTypes,
                Code = "code2",
                PromoPercent = 10,
                ExpirationDate = DateTime.Now.AddDays(1),
                MaxUsageTimes = 20,
                UsedTimes = 10,
            });

            await this.DbContext.SaveChangesAsync();


        }
    }
}
