using RestaurantMenuProject.Data.Common.Repositories;
using RestaurantMenuProject.Data.Models;
using RestaurantMenuProject.Services.Data.Contracts;
using RestaurantMenuProject.Services.Mapping;
using RestaurantMenuProject.Web.ViewModels;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RestaurantMenuProject.Services.Data
{
    public class OrderDrinkService : IOrderDrinkService
    {
        private readonly IRepository<OrderDrink> orderDrinkRepository;

        public OrderDrinkService(IRepository<OrderDrink> orderDrinkRepository)
        {
            this.orderDrinkRepository = orderDrinkRepository;
        }

        //public ICollection<FoodItemViewModel> GetAllDishesFromOrder(string orderId)
        //{
        //    var orderDrinks = this.orderDrinkRepository
        //        .AllAsNoTracking()
        //        .Where(x => x.OrderId == orderId)
        //        .To<FoodItemViewModel>()
        //        .ToList();
        //}
    }
}
