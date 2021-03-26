﻿namespace RestaurantMenuProject.Services.Data
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
        private readonly IPickupItemService pickupItemService;

        public OrderService(
            IDeletableEntityRepository<Order> orderRepository,
            IRepository<OrderDrink> orderDrinkRepository,
            IRepository<OrderDish> orderDishRepository,
            IBasketService basketService,
            IDishService dishService,
            IDrinkService drinkService,
            ITableService tableService,
            IPickupItemService pickupItemService
            )
        {
            this.orderRepository = orderRepository;
            this.orderDrinkRepository = orderDrinkRepository;
            this.orderDishRepository = orderDishRepository;
            this.basketService = basketService;
            this.dishService = dishService;
            this.drinkService = drinkService;
            this.tableService = tableService;
            this.pickupItemService = pickupItemService;
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
            return this.orderRepository.All().Where(x => x.WaiterId == waiterId).To<ActiveOrderViewModel>().ToList();
        }

        public WaiterViewModel GetWaiterViewModel(string userId)
        {
            var viewModel = new WaiterViewModel();
            viewModel.NewOrders = this.GetOrdersWithStatus(ProcessType.Pending);
            viewModel.PickupItems = this.pickupItemService.GetAllItemsToPickUp(); // Just make the method take userId and return only the waiters orders items
            viewModel.ActiveOrders = this.GetActiveOrders(userId);

            return viewModel;
        }
    }
}
