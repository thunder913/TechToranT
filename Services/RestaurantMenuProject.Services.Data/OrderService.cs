namespace RestaurantMenuProject.Services.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Dynamic.Core;
    using System.Threading.Tasks;
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
        private readonly IRepository<OrderDrink> orderDrinkRepository;
        private readonly IRepository<OrderDish> orderDishRepository;
        private readonly IBasketService basketService;
        private readonly IDishService dishService;
        private readonly IDrinkService drinkService;
        private readonly ITableService tableService;
        private readonly IDishTypeService dishTypeService;

        public OrderService(
            IDeletableEntityRepository<Order> orderRepository,
            IRepository<OrderDrink> orderDrinkRepository,
            IRepository<OrderDish> orderDishRepository,
            IBasketService basketService,
            IDishService dishService,
            IDrinkService drinkService,
            ITableService tableService,
            IDishTypeService dishTypeService
            )
        {
            this.orderRepository = orderRepository;
            this.orderDrinkRepository = orderDrinkRepository;
            this.orderDishRepository = orderDishRepository;
            this.basketService = basketService;
            this.dishService = dishService;
            this.drinkService = drinkService;
            this.tableService = tableService;
            this.dishTypeService = dishTypeService;
        }

        public ICollection<OrderInListViewModel> GetOrderViewModelsByUserId(int itemsPerPage, int page, string userId = null)
        {
            return this.orderRepository
                    .AllAsNoTrackingWithDeleted()
                    .Include(x => x.Client)
                    .OrderByDescending(x => x.CreatedOn)
                    .Skip((page - 1) * itemsPerPage)
                    .Take(itemsPerPage)
                    .Where(x => userId == null || x.ClientId == userId)
                    .To<OrderInListViewModel>()
                    .ToList();
        }

        public int GetUserOrdersCount(string userId)
        {
            return this.orderRepository.AllAsNoTrackingWithDeleted().Where(x => x.ClientId == userId).Count();
        }

        public Task MakeOrder(string userId, string tableCode)
        {
            var tableId = this.tableService.GetTableIdByCode(tableCode);
            var mapper = AutoMapperConfig.MapperInstance;
            BasketDto basket = this.basketService.GetBasket(userId);

            if (tableId == 0)
            {
                throw new Exception("The table code is invalid!");
            }

            // TODO use automapper
            if (!(basket.Drinks.Any() || basket.Dishes.Any()))
            {
                throw new ArgumentException("The basket is empty!");
            }

            Order order = new Order();
            order.ClientId = basket.Id;
            order.TableId = tableId;
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

            this.orderRepository.AddAsync(order).GetAwaiter().GetResult();


            this.orderRepository.SaveChangesAsync().GetAwaiter().GetResult();
            this.basketService.RemoveBasketItems(userId).GetAwaiter().GetResult();

            return Task.CompletedTask;
        }

        public async Task<bool> CancelOrder(string orderId)
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

            var orderDrinks = this.orderDrinkRepository
                .AllAsNoTracking()
                .Where(x => x.OrderId == orderId)
                .Select(x => new
                {
                    Id = x.DrinkId,
                    PriceForOne = x.PriceForOne,
                    Count = x.Count,
                })
                .ToList();

            foreach (var item in orderDrinks)
            {
                var drink = mapper.Map<Drink, FoodItemViewModel>(this.drinkService.GetDrinkWithDeletedById(item.Id));
                drink.Price = item.PriceForOne;
                drink.Quantity = item.Count;
                items.Add(drink);
            }

            var orderdishes = this.orderDishRepository
                .AllAsNoTracking()
                .Where(x => x.OrderId == orderId)
                .Select(x => new
                {
                    Id = x.DishId,
                    PriceForOne = x.PriceForOne,
                    Count = x.Count,
                })
                .ToList();

            foreach (var item in orderdishes)
            {
                var dish = mapper.Map<Dish, FoodItemViewModel>(this.dishService.GetDishWithDeletedById(item.Id));
                dish.Price = item.PriceForOne;
                dish.Quantity = item.Count;
                items.Add(dish);
            }

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

            if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortDirection)))
            {
                orders = orders.OrderBy(sortColumn + " " + sortDirection);
            }

            var dataToReturn = orders.To<ManageOrderViewModel>().ToList();

            if (!string.IsNullOrEmpty(searchValue))
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

        public void ChangeOrderStatus(ProcessType oldProcessType, ProcessType newProcessType, string orderId)
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
            this.orderRepository.SaveChangesAsync().GetAwaiter().GetResult();
        }

        public void AddWaiterToOrder(string orderId, string waiterId)
        {
            var order = this.orderRepository.All().FirstOrDefault(x => x.Id == orderId);
            if (order == null)
            {
                throw new InvalidOperationException("There is no such order!");
            }

            order.WaiterId = waiterId;
            this.orderRepository.SaveChangesAsync().GetAwaiter().GetResult();

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

        public async Task FinishOrder(string orderId)
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

        public ICollection<CookFoodCategoriesViewModel> GetCookFoodTypes()
        {
            var allDrinks = this.orderDrinkRepository
                .All()
                .Where(x => x.Order.ProcessType == ProcessType.Cooking && x.Count - x.DeliveredCount > 0)
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

            var dishTypes = this.orderDishRepository
                .All()
                .GroupBy(x => x.Dish.DishTypeId)
                .Select(x => x.Key);

            var dishes = new HashSet<CookFoodCategoriesViewModel>();

            foreach (var type in dishTypes)
            {
                var typeItems = this.orderDishRepository.All()
                    .Where(x => x.Dish.DishTypeId == type && x.Order.ProcessType == ProcessType.Cooking && x.Count - x.DeliveredCount > 0)
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


        public async Task AddDeliveredCountToOrderDish(int count, CookFinishItemViewModel itemViewModel)
        {
            var orderDishItem = this.orderDishRepository
                .All()
                .FirstOrDefault(x => x.OrderId == itemViewModel.OrderId && x.DishId == itemViewModel.FoodId);

            if (orderDishItem.Count - orderDishItem.DeliveredCount <= 0)
            {
                throw new InvalidOperationException("The item has already been made!");
            }

            orderDishItem.DeliveredCount += count;
            await this.orderDishRepository.SaveChangesAsync();
        }

        public async Task AddDeliveredCountToOrderDrink(int count, CookFinishItemViewModel itemViewModel)
        {
            var orderDrinkItem = this.orderDrinkRepository
                .All()
                .FirstOrDefault(x => x.OrderId == itemViewModel.OrderId && x.DrinkId == itemViewModel.FoodId);

            if (orderDrinkItem.Count - orderDrinkItem.DeliveredCount <= 0)
            {
                throw new InvalidOperationException("The item has already been made!");
            }

            orderDrinkItem.DeliveredCount += count;
            await this.orderDrinkRepository.SaveChangesAsync();
        }

        public PickupItem GetOrderDishAsPickupItem(CookFinishItemViewModel itemViewModel)
        {
            return this.orderDishRepository
                .All()
                .Where(x => x.OrderId == itemViewModel.OrderId && x.DishId == itemViewModel.FoodId)
                .Select(x => new PickupItem()
                 {
                     ClientName = x.Order.Client.FirstName + " " + x.Order.Client.LastName,
                     Name = x.Dish.Name,
                     TableNumber = x.Order.Table.Number,
                     WaiterId = x.Order.WaiterId,
                     Count = 1,
                     OrderId = itemViewModel.OrderId,
                 })
                .FirstOrDefault();
        }

        public PickupItem GetOrderDrinkAsPickupItem(CookFinishItemViewModel itemViewModel)
        {
            return this.orderDrinkRepository
                .All()
                .Where(x => x.OrderId == itemViewModel.OrderId && x.DrinkId == itemViewModel.FoodId)
                .Select(x => new PickupItem()
                {
                    ClientName = x.Order.Client.FirstName + " " + x.Order.Client.LastName,
                    Name = x.Drink.Name,
                    TableNumber = x.Order.Table.Number,
                    WaiterId = x.Order.WaiterId,
                    Count = 1,
                    OrderId = itemViewModel.OrderId,
                })
                .FirstOrDefault();
        }

        public double GetOrderDeliveredPerCent(string orderId)
        {
            var foodItems = new List<OrderDeliveredItemDto>();
            // Get all the items
            foodItems.AddRange(this.orderDishRepository
                .All()
                .Where(x => x.OrderId == orderId)
                .Select(x => new OrderDeliveredItemDto()
                {
                    Count = x.Count,
                    DeliveredCount = x.DeliveredCount,
                })
                .ToArray());
            foodItems.AddRange(this.orderDrinkRepository
                .All()
                .Where(x => x.OrderId == orderId)
                .Select(x => new OrderDeliveredItemDto()
                {
                    Count = x.Count,
                    DeliveredCount = x.DeliveredCount,
                })
                .ToArray());
            // TODO use automapper

            // Find the total delivered and ordered items
            var totalItemsOrdered = foodItems.Sum(x => x.Count);
            var totalItemsDelivered = foodItems.Sum(x => x.DeliveredCount);

            // Calculation the percent
            var percent = Math.Round((double)totalItemsDelivered / totalItemsOrdered * 100, 2);
            return percent;
        }
    }
}
