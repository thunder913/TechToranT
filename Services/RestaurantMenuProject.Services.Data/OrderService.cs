namespace RestaurantMenuProject.Services.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

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

        public OrderService(
            IDeletableEntityRepository<Order> orderRepository,
            IRepository<OrderDrink> orderDrinkRepository,
            IRepository<OrderDish> orderDishRepository,
            IBasketService basketService)
        {
            this.orderRepository = orderRepository;
            this.orderDrinkRepository = orderDrinkRepository;
            this.orderDishRepository = orderDishRepository;
            this.basketService = basketService;
        }

        public ICollection<OrderInListViewModel> GetOrderViewModelsByUserId(string userId, int itemsPerPage, int page)
        {
            return this.orderRepository
                    .AllAsNoTrackingWithDeleted()
                    .OrderByDescending(x => x.CreatedOn)
                    .Skip((page - 1) * itemsPerPage)
                    .Take(itemsPerPage)
                    .Where(x => x.ClientId == userId)
                    .To<OrderInListViewModel>()
                    .ToList();
        }

        public int GetUserOrdersCount(string userId)
        {
            return this.orderRepository.AllAsNoTrackingWithDeleted().Where(x => x.ClientId == userId).Count();
        }

        public Task MakeOrder(string userId)
        {
            var mapper = AutoMapperConfig.MapperInstance;
            BasketDto basket = this.basketService.GetBasket(userId);

            // TODO use automapper
            if (!(basket.Drinks.Any() || basket.Dishes.Any()))
            {
                throw new ArgumentException("The basket is empty!");
            }

            Order order = new Order();
            order.ClientId = basket.Id;
            order.OrderDrinks = basket.Drinks.Select(x => new OrderDrink()
            {
                Count = x.Quantity,
                DrinkId = x.Id,
            }).ToList();
            order.OrderDishes = basket.Dishes.Select(x => new OrderDish()
            {
                Count = x.Quantity,
                DishId = x.Id,
            }).ToList();

            this.orderRepository.AddAsync(order).GetAwaiter().GetResult();


            this.orderRepository.SaveChangesAsync().GetAwaiter().GetResult();
            this.basketService.RemoveBasketItems(userId).GetAwaiter().GetResult();

            return Task.CompletedTask;
        }

        public async Task<bool> DeleteById(string orderId)
        {
            var order = this.orderRepository.AllWithDeleted().FirstOrDefault(x => x.Id == orderId);

            if (order.ProcessType == ProcessType.Pending)
            {
                order.ProcessType = ProcessType.Cancelled;
                this.orderRepository.Delete(order);
                await this.orderRepository.SaveChangesAsync();
                return true;
            }

            return false;
        }

        public ICollection<FoodItemViewModel> GetAllDishesInOrder(string orderId)
        {
            var drinks = this.orderDrinkRepository
                .AllAsNoTracking()
                .Where(x => x.OrderId == orderId)
                .To<FoodItemViewModel>();

            var dishes = this.orderDishRepository
                .AllAsNoTracking()
                .Where(x => x.OrderId == orderId)
                .To<FoodItemViewModel>();

            var items = new List<FoodItemViewModel>();
            items.AddRange(drinks);
            items.AddRange(dishes);

            return items;
        }

        public OrderInfoViewModel GetFullInformationForOrder(string orderId)
        {
            var order = this.orderRepository
                .AllAsNoTrackingWithDeleted()
                .To<OrderInfoViewModel>()
                .FirstOrDefault(x => x.Id == orderId);

            order.FoodItems = this.GetAllDishesInOrder(orderId);

            return order;
        }
    }
}
