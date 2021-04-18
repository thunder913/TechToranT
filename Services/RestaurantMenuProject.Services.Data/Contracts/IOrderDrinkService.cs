using RestaurantMenuProject.Data.Models;
using RestaurantMenuProject.Data.Models.Dtos;
using RestaurantMenuProject.Web.ViewModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RestaurantMenuProject.Services.Data.Contracts
{
    public interface IOrderDrinkService
    {
        public ICollection<FoodItemViewModel> GetAllDrinksInOrderAsFoodItemViewModel(string orderId);

        public ICollection<OrderDeliveredItemDto> GetDrinksAsOrderDeliveredItemById(string orderId);

        public Task AddDeliveredCountToOrderDrinkAsync(string orderId, string foodId, int count);

        public ICollection<CookItemViewModel> GetAllCookingDrinksInOrder(string orderId);

        public ICollection<AnalyseCountViewModel> GetDrinksForWaiterAnalyse(string waiterId, DateTime startDate);

        public PickupItem GetOrderDrinkAsPickupItem(string foodId, string orderId);

        public bool AreAllDrinksDelivered(string orderId);

        public ICollection<SalesChartViewModel> GetDailyDrinkIncomeByPeriod(DateTime startDate, DateTime endDate);

        public ICollection<SalesChartViewModel> GetMonthlyDrinkIncomeByPeriod(DateTime startDate, DateTime endDate);

        public ICollection<SalesChartViewModel> GetYearlyDrinkIncomeByPeriod(DateTime startDate, DateTime endDate);
    }
}
