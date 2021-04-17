using RestaurantMenuProject.Data.Common.Repositories;
using RestaurantMenuProject.Data.Models;
using RestaurantMenuProject.Services.Data.Contracts;

namespace RestaurantMenuProject.Services.Data
{
    public class OrderDishService : IOrderDishService
    {
        private readonly IRepository<OrderDish> orderDishRepository;

        public OrderDishService(IRepository<OrderDish> orderDishRepository)
        {
            this.orderDishRepository = orderDishRepository;
        }
    }
}
