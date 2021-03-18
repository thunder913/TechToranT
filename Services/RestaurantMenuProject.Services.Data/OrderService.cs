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
        private readonly IBasketService basketService;

        public OrderService(
            IDeletableEntityRepository<Order> orderRepository,
            IBasketService basketService)
        {
            this.orderRepository = orderRepository;
            this.basketService = basketService;
        }

        public ICollection<OrderViewModel> GetOrderViewModelsByUserId(string userId)
        {
            return this.orderRepository
                    .All()
                    .Where(x => x.ClientId == userId)
                    .OrderByDescending(x => x.CreatedOn)
                    .To<OrderViewModel>()
                    .ToList();
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
            var order = this.orderRepository.All().FirstOrDefault(x => x.Id == orderId);

            if (order.ProcessType == ProcessType.Pending)
            {
                this.orderRepository.Delete(order);
                await this.orderRepository.SaveChangesAsync();
                return true;
            }

            return false;
        }
    }
}
