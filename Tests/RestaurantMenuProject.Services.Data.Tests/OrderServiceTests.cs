namespace RestaurantMenuProject.Services.Data.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
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
            var orderDishes = this.DbContext.OrderDishes
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

            var dishTypes = this.DbContext.OrderDishes
                .GroupBy(x => x.Dish.DishTypeId)
                .Select(x => x.Key);
            var dishes = new HashSet<CookFoodCategoriesViewModel>();

            foreach (var type in dishTypes)
            {
                var typeDishes = this.DbContext.OrderDishes.Where(x => (x.Dish.DishTypeId == type && x.Order.ProcessType == ProcessType.Cooking && x.Count - x.DeliveredCount > 0) && (orderId == null || x.OrderId == orderId))
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
        public async Task GetSalesDataForPeriodWorksCorrectlyWithDaily()
        {
            await this.PopulateDB();
            var orders = this.DbContext.Orders.ToList();
            orders[0].DeliveredOn = DateTime.UtcNow;
            orders[1].DeliveredOn = DateTime.UtcNow.AddDays(-1);
            orders[2].DeliveredOn = DateTime.UtcNow.AddDays(1);
            await this.DbContext.SaveChangesAsync();
            var startDate = DateTime.UtcNow.AddDays(-3);
            var endDate = DateTime.UtcNow.AddDays(3);
            var dishIncome = this.DbContext.OrderDishes
                    .Where(x => x.Order.DeliveredOn.Value.Date >= startDate.Date && x.Order.DeliveredOn.Value.Date <= endDate.Date)
                    .GroupBy(x => new { x.Order.DeliveredOn.Value.Day, x.Order.DeliveredOn.Value.Month, x.Order.DeliveredOn.Value.Year })
                    .Select(x => new SalesChartViewModel()
                    {
                        Date = new DateTime(x.Key.Year, x.Key.Month, x.Key.Day).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture),
                        Income = x.Sum(y => y.PriceForOne * y.Count),
                    }).ToList();
            var drinkIncome = this.DbContext.OrderDrinks
                .Where(x => x.Order.DeliveredOn.Value.Date >= startDate.Date && x.Order.DeliveredOn.Value.Date <= endDate.Date)
                    .GroupBy(x => new { x.Order.DeliveredOn.Value.Day, x.Order.DeliveredOn.Value.Month, x.Order.DeliveredOn.Value.Year })
                    .Select(x => new SalesChartViewModel()
                    {
                        Date = new DateTime(x.Key.Year, x.Key.Month, x.Key.Day).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture),
                        Income = x.Sum(y => y.PriceForOne * y.Count),
                    }).ToList();
            var dates = new List<string>();
            for (var dt = startDate; dt <= endDate; dt = dt.AddDays(1))
            {
                dates.Add(dt.ToString("dd/MM/yyyy"));
            }

            var expected = new SalesViewModel();
            foreach (var date in dates)
            {
                var dishIncomeToday = dishIncome.FirstOrDefault(x => x.Date == date);
                var drinkIncomeToday = drinkIncome.FirstOrDefault(x => x.Date == date);

                expected.DishIncome.Add(new SalesChartViewModel()
                {
                    Date = date,
                    Income = dishIncomeToday == null ? 0 : dishIncomeToday.Income,
                });

                expected.DrinkIncome.Add(new SalesChartViewModel()
                {
                    Date = date,
                    Income = drinkIncomeToday == null ? 0 : drinkIncomeToday.Income,
                });

                decimal totalIncome = 0;
                if (dishIncomeToday != null)
                {
                    totalIncome += dishIncomeToday.Income;
                }

                if (drinkIncomeToday != null)
                {
                    totalIncome += drinkIncomeToday.Income;
                }

                expected.TotalIncome.Add(new SalesChartViewModel()
                {
                    Date = date,
                    Income = totalIncome,
                });
            }

            var actual = this.OrderService.GetSalesDataForPeriod(startDate, endDate, "Daily");

            actual.IsDeepEqual(expected);
        }

        [Fact]
        public async Task GetSalesDataForPeriodWorksCorrectlyWithMonthly()
        {
            await this.PopulateDB();
            var orders = this.DbContext.Orders.ToList();
            orders[0].DeliveredOn = DateTime.UtcNow;
            orders[1].DeliveredOn = DateTime.UtcNow.AddMonths(-1);
            orders[2].DeliveredOn = DateTime.UtcNow.AddMonths(1);
            await this.DbContext.SaveChangesAsync();
            var startDate = DateTime.UtcNow.AddMonths(-3);
            var endDate = DateTime.UtcNow.AddMonths(3);
            var dishIncome = this.DbContext.OrderDishes
                    .Where(x => (x.Order.DeliveredOn >= startDate && x.Order.DeliveredOn < endDate)
                    || (x.Order.DeliveredOn.Value.Month == startDate.Month && x.Order.DeliveredOn.Value.Year == startDate.Year)
                    || (x.Order.DeliveredOn.Value.Month == endDate.Month && x.Order.DeliveredOn.Value.Year == endDate.Year))
                    .GroupBy(x => new { x.Order.DeliveredOn.Value.Month, x.Order.DeliveredOn.Value.Year })
                    .Select(x => new SalesChartViewModel()
                    {
                        Date = new DateTime(x.Key.Year, x.Key.Month, 1).ToString("MM/yyyy", CultureInfo.InvariantCulture),
                        Income = x.Sum(y => y.PriceForOne * y.Count),
                    }).ToList();
            var drinkIncome = this.DbContext.OrderDrinks
                .Where(x => (x.Order.DeliveredOn >= startDate && x.Order.DeliveredOn < endDate)
                    || (x.Order.DeliveredOn.Value.Month == startDate.Month && x.Order.DeliveredOn.Value.Year == startDate.Year)
                    || (x.Order.DeliveredOn.Value.Month == endDate.Month && x.Order.DeliveredOn.Value.Year == endDate.Year))
                    .GroupBy(x => new { x.Order.DeliveredOn.Value.Month, x.Order.DeliveredOn.Value.Year })
                    .Select(x => new SalesChartViewModel()
                    {
                        Date = new DateTime(x.Key.Year, x.Key.Month, 1).ToString("MM/yyyy", CultureInfo.InvariantCulture),
                        Income = x.Sum(y => y.PriceForOne * y.Count),
                    }).ToList();
            var dates = new List<string>();
            for (var dt = startDate; dt.Year < endDate.Year || (dt.Year <= endDate.Year && dt.Month <= endDate.Month); dt = dt.AddMonths(1))
            {
                dates.Add(dt.ToString("MM/yyyy"));
            }

            var expected = new SalesViewModel();
            foreach (var date in dates)
            {
                var dishIncomeToday = dishIncome.FirstOrDefault(x => x.Date == date && x.Date == date);
                var drinkIncomeToday = drinkIncome.FirstOrDefault(x => x.Date == date && x.Date == date);

                expected.DishIncome.Add(new SalesChartViewModel()
                {
                    Date = date,
                    Income = dishIncomeToday == null ? 0 : dishIncomeToday.Income,
                });

                expected.DrinkIncome.Add(new SalesChartViewModel()
                {
                    Date = date,
                    Income = drinkIncomeToday == null ? 0 : drinkIncomeToday.Income,
                });

                decimal totalIncome = 0;
                if (dishIncomeToday != null)
                {
                    totalIncome += dishIncomeToday.Income;
                }

                if (drinkIncomeToday != null)
                {
                    totalIncome += drinkIncomeToday.Income;
                }

                expected.TotalIncome.Add(new SalesChartViewModel()
                {
                    Date = date,
                    Income = totalIncome,
                });
            }

            var actual = this.OrderService.GetSalesDataForPeriod(startDate, endDate, "Monthly");

            actual.IsDeepEqual(expected);
        }

        [Fact]
        public async Task GetSalesDataForPeriodWorksCorrectlyWithHigherDateRange()
        {
            await this.PopulateDB();
            var orders = this.DbContext.Orders.ToList();
            orders[0].DeliveredOn = DateTime.UtcNow;
            orders[1].DeliveredOn = DateTime.UtcNow.AddMonths(-1);
            orders[2].DeliveredOn = DateTime.UtcNow.AddMonths(1);
            await this.DbContext.SaveChangesAsync();
            var startDate = DateTime.UtcNow.AddMonths(-12);
            var endDate = DateTime.UtcNow.AddMonths(12);
            var dishIncome = this.DbContext.OrderDishes
                    .Where(x => (x.Order.DeliveredOn >= startDate && x.Order.DeliveredOn < endDate)
                    || (x.Order.DeliveredOn.Value.Month == startDate.Month && x.Order.DeliveredOn.Value.Year == startDate.Year)
                    || (x.Order.DeliveredOn.Value.Month == endDate.Month && x.Order.DeliveredOn.Value.Year == endDate.Year))
                    .GroupBy(x => new { x.Order.DeliveredOn.Value.Month, x.Order.DeliveredOn.Value.Year })
                    .Select(x => new SalesChartViewModel()
                    {
                        Date = new DateTime(x.Key.Year, x.Key.Month, 1).ToString("MM/yyyy", CultureInfo.InvariantCulture),
                        Income = x.Sum(y => y.PriceForOne * y.Count),
                    }).ToList();
            var drinkIncome = this.DbContext.OrderDrinks
                .Where(x => (x.Order.DeliveredOn >= startDate && x.Order.DeliveredOn < endDate)
                    || (x.Order.DeliveredOn.Value.Month == startDate.Month && x.Order.DeliveredOn.Value.Year == startDate.Year)
                    || (x.Order.DeliveredOn.Value.Month == endDate.Month && x.Order.DeliveredOn.Value.Year == endDate.Year))
                    .GroupBy(x => new { x.Order.DeliveredOn.Value.Month, x.Order.DeliveredOn.Value.Year })
                    .Select(x => new SalesChartViewModel()
                    {
                        Date = new DateTime(x.Key.Year, x.Key.Month, 1).ToString("MM/yyyy", CultureInfo.InvariantCulture),
                        Income = x.Sum(y => y.PriceForOne * y.Count),
                    }).ToList();
            var dates = new List<string>();
            for (var dt = startDate; dt.Year < endDate.Year || (dt.Year <= endDate.Year && dt.Month <= endDate.Month); dt = dt.AddMonths(1))
            {
                dates.Add(dt.ToString("MM/yyyy"));
            }

            var expected = new SalesViewModel();
            foreach (var date in dates)
            {
                var dishIncomeToday = dishIncome.FirstOrDefault(x => x.Date == date && x.Date == date);
                var drinkIncomeToday = drinkIncome.FirstOrDefault(x => x.Date == date && x.Date == date);

                expected.DishIncome.Add(new SalesChartViewModel()
                {
                    Date = date,
                    Income = dishIncomeToday == null ? 0 : dishIncomeToday.Income,
                });

                expected.DrinkIncome.Add(new SalesChartViewModel()
                {
                    Date = date,
                    Income = drinkIncomeToday == null ? 0 : drinkIncomeToday.Income,
                });

                decimal totalIncome = 0;
                if (dishIncomeToday != null)
                {
                    totalIncome += dishIncomeToday.Income;
                }

                if (drinkIncomeToday != null)
                {
                    totalIncome += drinkIncomeToday.Income;
                }

                expected.TotalIncome.Add(new SalesChartViewModel()
                {
                    Date = date,
                    Income = totalIncome,
                });
            }

            var actual = this.OrderService.GetSalesDataForPeriod(startDate, endDate, "Monthly");

            actual.IsDeepEqual(expected);
        }

        [Fact]
        public async Task GetSalesDataForPeriodWorksCorrectlyWithYearly()
        {
            await this.PopulateDB();
            var orders = this.DbContext.Orders.ToList();
            orders[0].DeliveredOn = DateTime.UtcNow;
            orders[1].DeliveredOn = DateTime.UtcNow.AddYears(-1);
            orders[2].DeliveredOn = DateTime.UtcNow.AddYears(1);
            await this.DbContext.SaveChangesAsync();
            var startDate = DateTime.UtcNow.AddYears(-3);
            var endDate = DateTime.UtcNow.AddYears(3);
            var dishIncome = this.DbContext.OrderDishes
                    .Where(x => x.Order.DeliveredOn.Value.Year >= startDate.Year && x.Order.DeliveredOn.Value.Year <= endDate.Year)
                    .GroupBy(x => new { x.Order.DeliveredOn.Value.Year })
                    .Select(x => new SalesChartViewModel()
                    {
                        Date = new DateTime(x.Key.Year, 1, 1).ToString("yyyy", CultureInfo.InvariantCulture),
                        Income = x.Sum(y => y.PriceForOne * y.Count),
                    }).ToList();
            var drinkIncome = this.DbContext.OrderDrinks
                .Where(x => x.Order.DeliveredOn.Value.Year >= startDate.Year && x.Order.DeliveredOn.Value.Year <= endDate.Year)
                    .GroupBy(x => new { x.Order.DeliveredOn.Value.Year })
                    .Select(x => new SalesChartViewModel()
                    {
                        Date = new DateTime(x.Key.Year, 1, 1).ToString("yyyy", CultureInfo.InvariantCulture),
                        Income = x.Sum(y => y.PriceForOne * y.Count),
                    }).ToList();
            var dates = new List<string>();
            for (var dt = startDate; dt.Year <= endDate.Year; dt = dt.AddYears(1))
            {
                dates.Add(dt.ToString("yyyy"));
            }

            var expected = new SalesViewModel();
            foreach (var date in dates)
            {
                var dishIncomeToday = dishIncome.FirstOrDefault(x => x.Date == date);
                var drinkIncomeToday = drinkIncome.FirstOrDefault(x => x.Date == date);

                expected.DishIncome.Add(new SalesChartViewModel()
                {
                    Date = date,
                    Income = dishIncomeToday == null ? 0 : dishIncomeToday.Income,
                });

                expected.DrinkIncome.Add(new SalesChartViewModel()
                {
                    Date = date,
                    Income = drinkIncomeToday == null ? 0 : drinkIncomeToday.Income,
                });

                decimal totalIncome = 0;
                if (dishIncomeToday != null)
                {
                    totalIncome += dishIncomeToday.Income;
                }

                if (drinkIncomeToday != null)
                {
                    totalIncome += drinkIncomeToday.Income;
                }

                expected.TotalIncome.Add(new SalesChartViewModel()
                {
                    Date = date,
                    Income = totalIncome,
                });
            }

            var actual = this.OrderService.GetSalesDataForPeriod(startDate, endDate, "Yearly");

            actual.IsDeepEqual(expected);
        }

        [Fact]
        public void GetSalesDataForPeriodThrowsWhenGivenInvalidPeriod()
        {
            Assert.Throws<InvalidOperationException>(() => this.OrderService.GetSalesDataForPeriod(DateTime.UtcNow.AddDays(-1), DateTime.UtcNow, "invalid"));
        }





        [Fact]
        public async Task GetOrderDeliveredPerCentWorksCorrectly()
        {
            await this.PopulateDB();
            var orderId = "order2";

            var expected = 38.1;
            var actual = this.OrderService.GetOrderDeliveredPerCent(orderId);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task GetAllStaffForAnalyseAsyncWorksCorrectly()
        {
            await this.PopulateDB();
            var startDate = DateTime.UtcNow.AddYears(-1);
            var dates = new List<string>();
            var expected = new List<StaffAnalyseViewModel>();
            var allOrders = this.DbContext.Orders.ToList();
            allOrders[0].DeliveredOn = DateTime.UtcNow;
            allOrders[1].DeliveredOn = DateTime.UtcNow.AddMonths(-1);
            allOrders[2].DeliveredOn = DateTime.UtcNow.AddMonths(1);
            allOrders[0].ProcessType = ProcessType.Completed;
            allOrders[1].ProcessType = ProcessType.Completed;
            allOrders[2].ProcessType = ProcessType.Completed;
            await this.DbContext.SaveChangesAsync();
            for (var dt = startDate; dt.Year < startDate.Year || (dt.Year <= startDate.Year && dt.Month <= startDate.Month); dt = dt.AddMonths(1))
            {
                dates.Add(dt.ToString("MM/yyyy"));
            }

            var waiterIds =
                this.DbContext.Orders
                    .Where(x => x.CreatedOn >= startDate && x.ProcessType == ProcessType.Completed)
                    .GroupBy(x => x.WaiterId)
                    .Select(x => x.Key)
                    .ToList();

            foreach (var id in waiterIds)
            {
                var waiterOrderCount = await this.DbContext.Orders
                    .Where(x => x.ProcessType == ProcessType.Completed && x.WaiterId == id && ((x.CreatedOn >= startDate)
                    || (x.CreatedOn.Month == startDate.Month && x.CreatedOn.Year == startDate.Year)))
                    .GroupBy(x => new { x.CreatedOn.Year, x.CreatedOn.Month })
                    .Select(x => new
                    {
                        Date = new DateTime(x.Key.Year, x.Key.Month, 1).ToString("MM/yyyy", CultureInfo.InvariantCulture),
                        OrdersCount = x.Count(),
                    })
                    .ToListAsync();

                var dishesCount = await this.DbContext.OrderDishes
                    .Where(x => x.Order.ProcessType == ProcessType.Completed && x.Order.WaiterId == id && ((x.Order.CreatedOn >= startDate)
                    || (x.Order.CreatedOn.Month == startDate.Month && x.Order.CreatedOn.Year == startDate.Year)))
                    .GroupBy(x => new { x.Order.CreatedOn.Year, x.Order.CreatedOn.Month })
                    .Select(x => new
                    {
                        Date = new DateTime(x.Key.Year, x.Key.Month, 1).ToString("MM/yyyy", CultureInfo.InvariantCulture),
                        DishesCount = x.Sum(y => y.DeliveredCount),
                    })
                    .ToListAsync();

                var drinksCount = await this.DbContext.OrderDrinks
                    .Where(x => x.Order.ProcessType == ProcessType.Completed && x.Order.WaiterId == id && ((x.Order.CreatedOn >= startDate)
                    || (x.Order.CreatedOn.Month == startDate.Month && x.Order.CreatedOn.Year == startDate.Year)))
                    .GroupBy(x => new { x.Order.CreatedOn.Year, x.Order.CreatedOn.Month })
                    .Select(x => new
                    {
                        Date = new DateTime(x.Key.Year, x.Key.Month, 1).ToString("MM/yyyy", CultureInfo.InvariantCulture),
                        DrinksCount = x.Sum(y => y.DeliveredCount),
                    })
                    .ToListAsync();

                var waiterName = this.DbContext.Orders
                    .Where(x => x.WaiterId == id)
                    .Select(x => x.Waiter.FirstName + " " + x.Waiter.LastName)
                    .FirstOrDefault();

                var staffToAdd = new StaffAnalyseViewModel()
                {
                    FullName = waiterName,
                };

                foreach (var date in dates)
                {
                    var ordersThisMonth = waiterOrderCount.FirstOrDefault(x => x.Date == date);

                    var dishesThisMonth = dishesCount.FirstOrDefault(x => x.Date == date);

                    var drinksThisMonth = drinksCount.FirstOrDefault(x => x.Date == date);

                    var orders = 0;
                    var totalItemsDelivered = 0;

                    if (dishesThisMonth != null)
                    {
                        totalItemsDelivered += dishesThisMonth.DishesCount;
                    }

                    if (drinksThisMonth != null)
                    {
                        totalItemsDelivered += drinksThisMonth.DrinksCount;
                    }

                    if (ordersThisMonth != null)
                    {
                        orders += ordersThisMonth.OrdersCount;
                    }

                    var orderInfo = new StaffAnalyseOrdersViewModel()
                    {
                        Date = date,
                        ItemsDeliveredCount = totalItemsDelivered,
                        OrdersCount = orders,
                    };
                    staffToAdd.OrdersData.Add(orderInfo);
                }

                expected.Add(staffToAdd);
            }

            var actual = await this.OrderService.GetAllStaffForAnalyseAsync(startDate);

            actual.IsDeepEqual(expected);
        }

        [Fact]
        public async Task GetOrderInListByIdWorksCorrectly()
        {
            await this.PopulateDB();
            var orderId = this.DbContext.Orders.Skip(1).FirstOrDefault().Id;

            var expected = this.DbContext.Orders
                .To<OrderInListViewModel>()
                .FirstOrDefault(x => x.Id == orderId);
            expected.StatusName = Enum.GetName(typeof(ProcessType), expected.Status);
            var actual = this.OrderService.GetOrderInListById(orderId);

            actual.IsDeepEqual(expected);
        }

        [Fact]
        public async Task GetWaiterIdWorksCorrectly()
        {
            await this.PopulateDB();
            var orderId = "order2";

            var expected = "user2";
            var actual = this.OrderService.GetWaiterId(orderId);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task GetActiveOrderByIdWorksCorrectly()
        {
            await this.PopulateDB();
            var orderId = "order2";
            this.DbContext.Orders.FirstOrDefault(x => x.Id == orderId).ProcessType = ProcessType.Cooking;
            await this.DbContext.SaveChangesAsync();
            var expected = this.DbContext.Orders.To<ActiveOrderViewModel>().FirstOrDefault(x => x.Id == orderId);

            expected.ReadyPercent = 38.1;
            var actual = this.OrderService.GetActiveOrderById(orderId);

            actual.IsDeepEqual(expected);
        }

        [Fact]
        public async Task IsOrderCookedWorksCorrectlyWithFalse()
        {
            await this.PopulateDB();
            var orderId = "order2";

            var actual = this.OrderService.IsOrderCooked(orderId);

            Assert.False(actual);
        }

        [Fact]
        public async Task IsOrderCookedWorksCorrectlyWithTrue()
        {
            await this.PopulateDB();
            var orderId = "order3";

            var actual = this.OrderService.IsOrderCooked(orderId);

            Assert.True(actual);
        }

        [Fact]
        public async Task IsOrderPaidWorksCorrectly()
        {
            await this.PopulateDB();
            var notPaidOrder = this.DbContext.Orders.FirstOrDefault();
            var paidOrder = this.DbContext.Orders.Skip(1).FirstOrDefault();
            paidOrder.PaidOn = DateTime.UtcNow;
            await this.DbContext.SaveChangesAsync();

            var notPaidActual = this.OrderService.IsOrderPaid(notPaidOrder.Id);
            var paidActual = this.OrderService.IsOrderPaid(paidOrder.Id);

            Assert.False(notPaidActual);
            Assert.True(paidActual);
        }

        [Fact]
        public async Task PayOrderByIdAsyncWorksCorrectly()
        {
            await this.PopulateDB();
            var order = this.DbContext.Orders.FirstOrDefault();

            await this.OrderService.PayOrderByIdAsync(order.Id);

            Assert.NotNull(order.PaidOn);
        }

        [Fact]
        public async Task PayOrderByIdAsyncThrowsWhenGivenInvalidId()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(async () => await this.OrderService.PayOrderByIdAsync("Invalid"));
        }

        [Fact]
        public async Task PayOrderByIdAsyncThrowsWhenGivenPaidOrder()
        {
            await this.PopulateDB();
            var order = this.DbContext.Orders.FirstOrDefault();
            order.PaidOn = DateTime.UtcNow;
            await this.DbContext.SaveChangesAsync();

            await Assert.ThrowsAsync<InvalidOperationException>(async () => await this.OrderService.PayOrderByIdAsync(order.Id));
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

            await this.DbContext.OrderDishes.AddAsync(new OrderDish()
            {
                OrderId = "order1",
                Count = 3,
                DeliveredCount = 1,
                DishId = "test2",
                PriceForOne = 10,
            });

            await this.DbContext.OrderDishes.AddAsync(new OrderDish()
            {
                OrderId = "order2",
                Count = 3,
                DeliveredCount = 1,
                DishId = "test2",
                PriceForOne = 10,
            });

            await this.DbContext.OrderDishes.AddAsync(new OrderDish()
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
