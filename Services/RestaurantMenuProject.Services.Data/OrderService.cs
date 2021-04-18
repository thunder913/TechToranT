namespace RestaurantMenuProject.Services.Data
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Linq.Dynamic.Core;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;
    using RestaurantMenuProject.Data.Common.Repositories;
    using RestaurantMenuProject.Data.Models;
    using RestaurantMenuProject.Data.Models.Dtos;
    using RestaurantMenuProject.Data.Models.Enums;
    using RestaurantMenuProject.Services.Data.Contracts;
    using RestaurantMenuProject.Services.Mapping;
    using RestaurantMenuProject.Web.ViewModels;

    public class OrderService : IOrderService
    {
        private readonly IDeletableEntityRepository<Order> orderRepository;
        private readonly IBasketService basketService;
        private readonly ITableService tableService;
        private readonly IDishTypeService dishTypeService;
        private readonly IPromoCodeService promoCodeService;
        private readonly IOrderDishService orderDishService;
        private readonly IOrderDrinkService orderDrinkService;

        public OrderService(
            IDeletableEntityRepository<Order> orderRepository,
            IBasketService basketService,
            ITableService tableService,
            IDishTypeService dishTypeService,
            IPromoCodeService promoCodeService,
            IOrderDishService orderDishService,
            IOrderDrinkService orderDrinkService
            )
        {
            this.orderRepository = orderRepository;
            this.basketService = basketService;
            this.tableService = tableService;
            this.dishTypeService = dishTypeService;
            this.promoCodeService = promoCodeService;
            this.orderDishService = orderDishService;
            this.orderDrinkService = orderDrinkService;
        }

        public ICollection<OrderInListViewModel> GetOrderViewModelsByUserId(int itemsPerPage, int page, string userId = null)
        {
            return this.orderRepository
                    .AllAsNoTrackingWithDeleted()
                    .Where(x => userId == null || x.ClientId == userId)
                    .Include(x => x.Client)
                    .OrderByDescending(x => x.CreatedOn)
                    .Skip((page - 1) * itemsPerPage)
                    .Take(itemsPerPage)
                    .To<OrderInListViewModel>()
                    .ToList();
        }

        public int GetUserOrdersCount(string userId)
        {
            return this.orderRepository.AllAsNoTrackingWithDeleted().Where(x => x.ClientId == userId).Count();
        }

        public async Task<string> MakeOrderAsync(string userId, string tableCode)
        {
            var tableId = this.tableService.GetTableIdByCode(tableCode);
            var mapper = AutoMapperConfig.MapperInstance;
            BasketDto basket = this.basketService.GetBasket(userId);

            // TODO use automapper
            if (!(basket.Drinks.Any() || basket.Dishes.Any()))
            {
                throw new Exception("The basket is empty!");
            }

            Order order = new Order();
            order.ClientId = basket.Id;
            order.TableId = tableId;

            if (basket.PromoCode != null)
            {
                foreach (var dish in basket.Dishes)
                {
                    if (basket.PromoCode.ValidDishCategories.Any(x => x.Name == dish.CategoryName))
                    {
                        dish.Price = Math.Round(dish.Price * (1 - (decimal.Parse(basket.PromoCode.PromoPercent.ToString()) / 100)), 2);
                    }
                }

                foreach (var drink in basket.Drinks)
                {
                    if (basket.PromoCode.ValidDrinkCategories.Any(x => x.Name == drink.CategoryName))
                    {
                        drink.Price = Math.Round(drink.Price * (1 - (decimal.Parse(basket.PromoCode.PromoPercent.ToString()) / 100)), 2);
                    }
                }

                await this.promoCodeService.UsePromoCodeAsync(basket.PromoCode.Id, 1);

                order.PromoCode = basket.PromoCode;
            }

            order.OrderDrinks = basket.Drinks.Select(x => new OrderDrink()
            {
                Count = x.Quantity,
                DrinkId = x.Id,
                PriceForOne = x.Price,
            }).ToList();

            order.OrderDishes = basket.Dishes.Select(x => new OrderDish()
            {
                Count = x.Quantity,
                DishId = x.Id,
                PriceForOne = x.Price,
            }).ToList();

            await this.orderRepository.AddAsync(order);
            await this.orderRepository.SaveChangesAsync();
            await this.basketService.RemoveBasketItemsAsync(userId);

            return order.Id;
        }

        public async Task<bool> CancelOrderAsync(string orderId)
        {
            var order = this.orderRepository.AllWithDeleted().FirstOrDefault(x => x.Id == orderId);

            if (order.ProcessType == ProcessType.Pending)
            {
                order.ProcessType = ProcessType.Cancelled;
                await this.orderRepository.SaveChangesAsync();
                return true;
            }

            return false;
        }

        public ICollection<FoodItemViewModel> GetAllFoodItemsById(string orderId)
        {
            var mapper = AutoMapperConfig.MapperInstance;
            var items = new List<FoodItemViewModel>();

            var orderDrinks = this.orderDrinkService.GetAllDrinksInOrderAsFoodItemViewModel(orderId);

            var orderDishes = this.orderDishService.GetAllDishesInOrderAsFoodItemViewModel(orderId);

            items.AddRange(orderDrinks);
            items.AddRange(orderDishes);

            return items;
        }

        public OrderInfoViewModel GetFullInformationForOrder(string orderId)
        {
            var order = this.orderRepository
                .AllAsNoTrackingWithDeleted()
                .To<OrderInfoViewModel>()
                .FirstOrDefault(x => x.Id == orderId);

            order.FoodItems = this.GetAllFoodItemsById(orderId);

            return order;
        }

        public ICollection<ManageOrderViewModel> GetAllOrders(string sortColumn, string sortDirection, string searchValue)
        {
            var orders = this.orderRepository
                .AllAsNoTrackingWithDeleted()
                .To<OrderInListViewModel>();

            if (!(string.IsNullOrWhiteSpace(sortColumn) || string.IsNullOrWhiteSpace(sortDirection)))
            {
                orders = orders.OrderBy(sortColumn + " " + sortDirection);
            }

            var dataToReturn = orders.To<ManageOrderViewModel>().ToList();

            if (!string.IsNullOrWhiteSpace(searchValue))
            {
                dataToReturn = dataToReturn.Where(m =>
                                            m.Price.ToString().Contains(searchValue)
                                            || m.Email.ToLower().Contains(searchValue.ToLower())
                                            || m.Status.ToString().ToLower().Contains(searchValue.ToLower())
                                            || m.Date.ToLocalTime().ToString("dd/MM/yyyy, HH:mm:ss").Contains(searchValue)
                                            || m.FullName.ToLower().Contains(searchValue)).ToList(); // TODO fix it again to make it do it all as Queryable
            }

            return dataToReturn;
        }

        public ICollection<OrderInListViewModel> GetOrdersWithStatus(ProcessType processType)
        {
            return this.orderRepository
                .All()
                .Where(x => x.ProcessType == processType)
                .OrderBy(x => x.CreatedOn)
                .To<OrderInListViewModel>()
                .ToList();
        }

        public async Task ChangeOrderStatusAsync(ProcessType oldProcessType, ProcessType newProcessType, string orderId)
        {
            if (oldProcessType == newProcessType)
            {
                throw new InvalidOperationException("The status is the same.");
            }

            var order = this.orderRepository.AllWithDeleted().Where(x => x.Id == orderId && x.ProcessType == oldProcessType).FirstOrDefault();

            if (order == null)
            {
                throw new InvalidOperationException("The old status has changed!");
            }

            order.ProcessType = newProcessType;

            this.orderRepository.Update(order);
            await this.orderRepository.SaveChangesAsync();
        }

        public async Task AddWaiterToOrderAsync(string orderId, string waiterId)
        {
            var order = this.orderRepository.All().FirstOrDefault(x => x.Id == orderId);
            if (order == null)
            {
                throw new InvalidOperationException("There is no such order!");
            }

            order.WaiterId = waiterId;
            await this.orderRepository.SaveChangesAsync();

        }

        public ICollection<ActiveOrderViewModel> GetActiveOrders(string waiterId)
        {
            var activeOrders = this.orderRepository
                .All()
                .Where(x => x.WaiterId == waiterId && x.ProcessType != ProcessType.Completed && x.ProcessType != ProcessType.Pending)
                .OrderBy(x => x.CreatedOn)
                .To<ActiveOrderViewModel>()
                .ToList();
            foreach (var order in activeOrders)
            {
                order.ReadyPercent = this.GetOrderDeliveredPerCent(order.Id);
            }

            return activeOrders;
        }

        public WaiterViewModel GetWaiterViewModel(string userId)
        {
            var viewModel = new WaiterViewModel();
            viewModel.NewOrders = this.GetOrdersWithStatus(ProcessType.Pending);
            viewModel.ActiveOrders = this.GetActiveOrders(userId);

            return viewModel;
        }

        public async Task FinishOrderAsync(string orderId)
        {
            var order = this.orderRepository.All().FirstOrDefault(x => x.Id == orderId);
            if (order.PaidOn == null)
            {
                throw new InvalidOperationException("The order is not paid!");
            }

            if (order.ProcessType != ProcessType.Delivered)
            {
                throw new InvalidOperationException("The order has not yet been delivered!");
            }

            order.ProcessType = ProcessType.Completed;
            await this.orderRepository.SaveChangesAsync();
        }

        public ChefViewModel GetChefViewModel()
        {
            var viewModel = new ChefViewModel();
            viewModel.NewOrders = this.GetOrdersWithStatus(ProcessType.InProcess);
            viewModel.FoodTypes = this.GetCookFoodTypes();
            return viewModel;
        }

        public ICollection<CookFoodCategoriesViewModel> GetCookFoodTypes(string orderId = null)
        {
            var allDrinks = this.orderDrinkService.GetAllCookingDrinksInOrder(orderId);

            var drinks = new CookFoodCategoriesViewModel()
            {
                FoodType = FoodType.Drink,
                CategoryName = "Drinks",
                ItemsToCook = allDrinks,
            };

            var dishTypes = this.orderDishService.GetDishTypeIds();

            var dishes = new HashSet<CookFoodCategoriesViewModel>();

            foreach (var type in dishTypes)
            {
                var typeItems = this.orderDishService.GetAllNotDeliveredCookedItemsByDishType(type, orderId);

                dishes.Add(new CookFoodCategoriesViewModel()
                {
                    CategoryName = this.dishTypeService.GetDishTypeById(type).Name,
                    FoodType = FoodType.Dish,
                    ItemsToCook = typeItems,
                });
            }

            var toReturn = new HashSet<CookFoodCategoriesViewModel>();

            toReturn.Add(drinks);

            foreach (var dish in dishes)
            {
                toReturn.Add(dish);
            }

            return toReturn;
        }

        public async Task AddDeliveredCountToOrderDishAsync(int count, CookFinishItemViewModel itemViewModel)
        {
            await this.orderDishService.AddDeliveredCountToOrderDishAsync(itemViewModel.OrderId, itemViewModel.FoodId, count);
        }

        public async Task AddDeliveredCountToOrderDrinkAsync(int count, CookFinishItemViewModel itemViewModel)
        {
            await this.orderDrinkService.AddDeliveredCountToOrderDishAsync(itemViewModel.OrderId, itemViewModel.FoodId, count);
        }

        public PickupItem GetOrderDishAsPickupItem(CookFinishItemViewModel itemViewModel)
        {
            return this.orderDishService.GetOrderDishAsPickupItem(itemViewModel.FoodId, itemViewModel.OrderId);
        }

        public PickupItem GetOrderDrinkAsPickupItem(CookFinishItemViewModel itemViewModel)
        {
            return this.orderDrinkService.GetOrderDrinkAsPickupItem(itemViewModel.FoodId, itemViewModel.OrderId);
        }

        public double GetOrderDeliveredPerCent(string orderId)
        {
            var foodItems = new List<OrderDeliveredItemDto>();
            // Get all the items
            foodItems.AddRange(this.orderDishService.GetDishesAsOrderDeliveredItemById(orderId));
            foodItems.AddRange(this.orderDrinkService.GetDrinksAsOrderDeliveredItemById(orderId));

            // TODO use automapper

            // Find the total delivered and ordered items
            var totalItemsOrdered = foodItems.Sum(x => x.Count);
            var totalItemsDelivered = foodItems.Sum(x => x.DeliveredCount);

            // Calculation the percent
            var percent = Math.Round((double)totalItemsDelivered / totalItemsOrdered * 100, 2);
            return percent;
        }

        public SalesViewModel GetSalesDataForPeriod(DateTime startDate, DateTime endDate, string period)
        {
            // Get the dates for the specific period
            var dates = new List<string>();
            var dishIncome = new List<SalesChartViewModel>();
            var drinkIncome = new List<SalesChartViewModel>();
            switch (period.ToLower())
            {
                case "daily":
                    for (var dt = startDate; dt <= endDate; dt = dt.AddDays(1))
                    {
                        dates.Add(dt.ToString("dd/MM/yyyy"));
                    }

                    dishIncome = this.orderDishService.GetDailyDishIncomeByPeriod(startDate, endDate).ToList();
                    drinkIncome = this.orderDrinkService.GetDailyDrinkIncomeByPeriod(startDate, endDate).ToList();
                    break;
                case "monthly":
                    for (var dt = startDate; dt.Year < endDate.Year || (dt.Year <= endDate.Year && dt.Month <= endDate.Month); dt = dt.AddMonths(1))
                    {
                        dates.Add(dt.ToString("MM/yyyy"));
                    }

                    dishIncome = this.orderDishService.GetMonthlyDishIncomeByPeriod(startDate, endDate).ToList();
                    drinkIncome = this.orderDrinkService.GetMonthlyDrinkIncomeByPeriod(startDate, endDate).ToList();
                    break;
                case "yearly":
                    for (var dt = startDate; dt.Year <= endDate.Year; dt = dt.AddYears(1))
                    {
                        dates.Add(dt.ToString("yyyy"));
                    }

                    dishIncome = this.orderDishService.GetYearlyDishIncomeByPeriod(startDate, endDate).ToList();
                    drinkIncome = this.orderDrinkService.GetYearlyDrinkIncomeByPeriod(startDate, endDate).ToList();
                    break;
                default:
                    throw new InvalidOperationException("An invalid period was given!");
            }

            // Return the sales
            return this.GetSales(dates, dishIncome, drinkIncome, period);

        }

        public async Task<ICollection<StaffAnalyseViewModel>> GetAllStaffForAnalyseAsync(DateTime startDate)
        {
            var dates = new List<string>();
            var currentDate = DateTime.UtcNow;
            for (var dt = startDate; dt.Year < currentDate.Year || (dt.Year <= currentDate.Year && dt.Month <= currentDate.Month); dt = dt.AddMonths(1))
            {
                dates.Add(dt.ToString("MM/yyyy"));
            }


            var waiterIds =
                this.orderRepository
                .All()
                .Where(x => x.CreatedOn >= startDate && x.ProcessType == ProcessType.Completed)
                .GroupBy(x => x.WaiterId)
                .Select(x => x.Key)
                .ToList();

            var waiters = new List<StaffAnalyseViewModel>();

            foreach (var id in waiterIds)
            {
                var waiterOrderCount = await this.orderRepository
                    .All()
                    .Where(x => x.ProcessType == ProcessType.Completed && x.WaiterId == id && ((x.CreatedOn >= startDate)
                    || (x.CreatedOn.Month == startDate.Month && x.CreatedOn.Year == startDate.Year)))
                    .GroupBy(x => new { x.CreatedOn.Year, x.CreatedOn.Month })
                    .Select(x => new
                    {
                        Date = new DateTime(x.Key.Year, x.Key.Month, 1).ToString("MM/yyyy", CultureInfo.InvariantCulture),
                        OrdersCount = x.Count(),
                    })
                    .ToListAsync();

                var dishesCount = this.orderDishService.GetDishesForWaiterAnalyse(id, startDate);

                var drinksCount = this.orderDrinkService.GetDrinksForWaiterAnalyse(id, startDate);

                var waiterName = this.orderRepository.All().Where(x => x.WaiterId == id).Select(x => x.Waiter.FirstName + " " + x.Waiter.LastName).FirstOrDefault();

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
                        totalItemsDelivered += dishesThisMonth.Count;
                    }

                    if (drinksThisMonth != null)
                    {
                        totalItemsDelivered += drinksThisMonth.Count;
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

                waiters.Add(staffToAdd);
            }

            return waiters;
        }

        public OrderInListViewModel GetOrderInListById(string id)
        {
            var order = this.orderRepository
                .All()
                .To<OrderInListViewModel>()
                .FirstOrDefault(x => x.Id == id);
            order.StatusName = Enum.GetName(typeof(ProcessType), order.Status);

            return order;
        }

        public string GetWaiterId(string id)
        {
            return this.orderRepository
                .All()
                .FirstOrDefault(x => x.Id == id)
                .WaiterId;
        }

        public ActiveOrderViewModel GetActiveOrderById(string id)
        {
            var order = this.orderRepository
                .All()
                .Where(x => x.ProcessType != ProcessType.Completed && x.ProcessType != ProcessType.Pending && x.Id == id)
                .To<ActiveOrderViewModel>()
                .FirstOrDefault();

            order.ReadyPercent = this.GetOrderDeliveredPerCent(id);

            return order;
        }

        public bool IsOrderCooked(string orderId)
        {
            var drinksDelivered = this.orderDrinkService.AreAllDrinksDelivered(orderId);

            var dishesDelivered = this.orderDishService.AreAllDishesDelivered(orderId);

            return drinksDelivered && dishesDelivered;

        }

        public bool IsOrderPaid(string orderId)
        {
            return this.orderRepository.All().FirstOrDefault(x => x.Id == orderId).PaidOn != null;
        }

        public async Task PayOrderByIdAsync(string id)
        {
            var order = this.orderRepository.All().FirstOrDefault(x => x.Id == id);
            if (order == null)
            {
                throw new ArgumentNullException("There is no order with the given id!");
            }

            if (order.PaidOn != null)
            {
                throw new InvalidOperationException("The given order is already paid!");
            }

            order.PaidOn = DateTime.UtcNow;

            await this.orderRepository.SaveChangesAsync();


        }

        private SalesViewModel GetSales(List<string> dates, ICollection<SalesChartViewModel> dishIncome, ICollection<SalesChartViewModel> drinkIncome, string period)
        {
            var salesViewModel = new SalesViewModel();
            foreach (var date in dates)
            {
                var dishIncomeToday = new SalesChartViewModel();
                var drinkIncomeToday = new SalesChartViewModel();

                // Get the right dish/drink if it exists
                switch (period.ToLower())
                {
                    case "daily":
                        dishIncomeToday = dishIncome.FirstOrDefault(x => x.Date == date);
                        drinkIncomeToday = drinkIncome.FirstOrDefault(x => x.Date == date);
                        break;
                    case "monthly":
                        dishIncomeToday = dishIncome.FirstOrDefault(x => x.Date == date && x.Date == date);
                        drinkIncomeToday = drinkIncome.FirstOrDefault(x => x.Date == date && x.Date == date);

                        break;
                    case "yearly":
                        dishIncomeToday = dishIncome.FirstOrDefault(x => x.Date == date);
                        drinkIncomeToday = drinkIncome.FirstOrDefault(x => x.Date == date);
                        break;
                }

                // Make new model
                salesViewModel.DishIncome.Add(new SalesChartViewModel()
                {
                    Date = date,
                    Income = dishIncomeToday == null ? 0 : dishIncomeToday.Income,
                });

                salesViewModel.DrinkIncome.Add(new SalesChartViewModel()
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

                salesViewModel.TotalIncome.Add(new SalesChartViewModel()
                {
                    Date = date,
                    Income = totalIncome,
                });
            }

            return salesViewModel;
        }
    }
}
