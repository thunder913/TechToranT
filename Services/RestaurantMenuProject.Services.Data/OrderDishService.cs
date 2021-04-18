using RestaurantMenuProject.Data.Common.Repositories;
using RestaurantMenuProject.Data.Models;
using RestaurantMenuProject.Data.Models.Dtos;
using RestaurantMenuProject.Data.Models.Enums;
using RestaurantMenuProject.Services.Data.Contracts;
using RestaurantMenuProject.Services.Mapping;
using RestaurantMenuProject.Web.ViewModels;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace RestaurantMenuProject.Services.Data
{
    public class OrderDishService : IOrderDishService
    {
        private readonly IRepository<OrderDish> orderDishRepository;

        public OrderDishService(IRepository<OrderDish> orderDishRepository)
        {
            this.orderDishRepository = orderDishRepository;
        }

        public ICollection<FoodItemViewModel> GetAllDishesInOrderAsFoodItemViewModel(string orderId)
        {
            return this.orderDishRepository
                .AllAsNoTracking()
                .Where(x => x.OrderId == orderId)
                .To<FoodItemViewModel>()
                .ToList();
        }

        public ICollection<int> GetDishTypeIds()
        {
            return this.orderDishRepository
                .All()
                .GroupBy(x => x.Dish.DishTypeId)
                .Select(x => x.Key)
                .ToList();
        }

        public ICollection<CookItemViewModel> GetAllNotDeliveredCookedItemsByDishType(int dishTypeId, string orderId)
        {
            return this.orderDishRepository.All()
                        .Where(x => (x.Dish.DishTypeId == dishTypeId && x.Order.ProcessType == ProcessType.Cooking && x.Count - x.DeliveredCount > 0) && (orderId == null || x.OrderId == orderId))
                        .OrderBy(x => x.Order.CreatedOn)
                        .Select(x => new CookItemViewModel()
                        {
                            OrderId = x.OrderId,
                            FoodId = x.Dish.Id,
                            Count = x.Count - x.DeliveredCount,
                            FoodName = x.Dish.Name,
                        }).ToList();
        }

        public async Task AddDeliveredCountToOrderDishAsync(string orderId, string foodId, int count)
        {
            var orderDishItem = this.orderDishRepository
                .All()
                .FirstOrDefault(x => x.OrderId == orderId && x.DishId == foodId);

            if (orderDishItem.Count - orderDishItem.DeliveredCount <= 0)
            {
                throw new InvalidOperationException("The item has already been made!");
            }

            orderDishItem.DeliveredCount += count;
            await this.orderDishRepository.SaveChangesAsync();
        }

        public PickupItem GetOrderDishAsPickupItem(string foodId, string orderId)
        {
            return this.orderDishRepository
                .All()
                .Where(x => x.OrderId == orderId && x.DishId == foodId)
                .Select(x => new PickupItem()
                {
                    ClientName = x.Order.Client.FirstName + " " + x.Order.Client.LastName,
                    Name = x.Dish.Name,
                    TableNumber = x.Order.Table.Number,
                    WaiterId = x.Order.WaiterId,
                    Count = 1,
                    OrderId = orderId,
                })
                .FirstOrDefault();
        }

        public ICollection<OrderDeliveredItemDto> GetDishesAsOrderDeliveredItemById(string orderId)
        {
            return this.orderDishRepository
                .All()
                .Where(x => x.OrderId == orderId)
                .Select(x => new OrderDeliveredItemDto()
                {
                    Count = x.Count,
                    DeliveredCount = x.DeliveredCount,
                })
                .ToArray();
        }

        public ICollection<AnalyseCountViewModel> GetDishesForWaiterAnalyse(string waiterId, DateTime startDate)
        {
            return this.orderDishRepository
                    .All()
                    .Where(x => x.Order.ProcessType == ProcessType.Completed && x.Order.WaiterId == waiterId && ((x.Order.CreatedOn >= startDate)
                    || (x.Order.CreatedOn.Month == startDate.Month && x.Order.CreatedOn.Year == startDate.Year)))
                    .GroupBy(x => new { x.Order.CreatedOn.Year, x.Order.CreatedOn.Month })
                    .Select(x => new AnalyseCountViewModel()
                    {
                        Date = new DateTime(x.Key.Year, x.Key.Month, 1).ToString("MM/yyyy", CultureInfo.InvariantCulture),
                        Count = x.Sum(y => y.DeliveredCount),
                    })
                    .ToList();
        }

        public bool AreAllDishesDelivered(string orderId)
        {
            return this.orderDishRepository
                .All()
                .Where(x => x.OrderId == orderId)
                .All(x => x.DeliveredCount >= x.Count);
        }

        public ICollection<SalesChartViewModel> GetDailyDishIncomeByPeriod(DateTime startDate, DateTime endDate)
        {
            return this.orderDishRepository
                    .All()
                    .Where(x => x.Order.DeliveredOn.Value.Date >= startDate.Date && x.Order.DeliveredOn.Value.Date <= endDate.Date)
                    .GroupBy(x => new { x.Order.DeliveredOn.Value.Day, x.Order.DeliveredOn.Value.Month, x.Order.DeliveredOn.Value.Year })
                    .Select(x => new SalesChartViewModel()
                    {
                        Date = new DateTime(x.Key.Year, x.Key.Month, x.Key.Day).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture),
                        Income = x.Sum(y => y.PriceForOne * y.Count),
                    }).ToList();
        }

        public ICollection<SalesChartViewModel> GetMonthlyDishIncomeByPeriod(DateTime startDate, DateTime endDate)
        {
            return this.orderDishRepository
                    .All()
                    .Where(x => (x.Order.DeliveredOn >= startDate && x.Order.DeliveredOn < endDate)
                    || (x.Order.DeliveredOn.Value.Month == startDate.Month && x.Order.DeliveredOn.Value.Year == startDate.Year)
                    || (x.Order.DeliveredOn.Value.Month == endDate.Month && x.Order.DeliveredOn.Value.Year == endDate.Year))
                    .GroupBy(x => new { x.Order.DeliveredOn.Value.Month, x.Order.DeliveredOn.Value.Year })
                    .Select(x => new SalesChartViewModel()
                    {
                        Date = new DateTime(x.Key.Year, x.Key.Month, 1).ToString("MM/yyyy", CultureInfo.InvariantCulture),
                        Income = x.Sum(y => y.PriceForOne * y.Count),
                    }).ToList();
        }

        public ICollection<SalesChartViewModel> GetYearlyDishIncomeByPeriod(DateTime startDate, DateTime endDate)
        {
            return this.orderDishRepository
                    .All()
                    .Where(x => x.Order.DeliveredOn.Value.Year >= startDate.Year && x.Order.DeliveredOn.Value.Year <= endDate.Year)
                    .GroupBy(x => new { x.Order.DeliveredOn.Value.Year })
                    .Select(x => new SalesChartViewModel()
                    {
                        Date = new DateTime(x.Key.Year, 1, 1).ToString("yyyy", CultureInfo.InvariantCulture),
                        Income = x.Sum(y => y.PriceForOne * y.Count),
                    }).ToList();
        }
    }
}
