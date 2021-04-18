using RestaurantMenuProject.Data.Models;
using RestaurantMenuProject.Data.Models.Dtos;
using RestaurantMenuProject.Web.ViewModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RestaurantMenuProject.Services.Data.Contracts
{
    public interface IOrderDishService
    {
        public ICollection<FoodItemViewModel> GetAllDishesInOrderAsFoodItemViewModel(string orderId);

        public ICollection<int> GetDishTypeIds();

        public ICollection<CookItemViewModel> GetAllNotDeliveredCookedItemsByDishType(int dishTypeId, string orderId);

        public Task AddDeliveredCountToOrderDishAsync(string orderId, string foodId, int count);

        public PickupItem GetOrderDishAsPickupItem(string foodId, string orderId);

        public ICollection<OrderDeliveredItemDto> GetDishesAsOrderDeliveredItemById(string orderId);

        public ICollection<AnalyseCountViewModel> GetDishesForWaiterAnalyse(string waiterId, DateTime startDate);

        public bool AreAllDishesDelivered(string orderId);

        public ICollection<SalesChartViewModel> GetDailyDishIncomeByPeriod(DateTime startDate, DateTime endDate);

        public ICollection<SalesChartViewModel> GetMonthlyDishIncomeByPeriod(DateTime startDate, DateTime endDate);

        public ICollection<SalesChartViewModel> GetYearlyDishIncomeByPeriod(DateTime startDate, DateTime endDate);
    }
}
