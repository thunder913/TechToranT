namespace RestaurantMenuProject.Services.Data
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Threading.Tasks;

    using RestaurantMenuProject.Data.Common.Repositories;
    using RestaurantMenuProject.Data.Models;
    using RestaurantMenuProject.Data.Models.Dtos;
    using RestaurantMenuProject.Data.Models.Enums;
    using RestaurantMenuProject.Services.Data.Contracts;
    using RestaurantMenuProject.Services.Mapping;
    using RestaurantMenuProject.Web.ViewModels;

    public class OrderDrinkService : IOrderDrinkService
    {
        private readonly IRepository<OrderDrink> orderDrinkRepository;

        public OrderDrinkService(IRepository<OrderDrink> orderDrinkRepository)
        {
            this.orderDrinkRepository = orderDrinkRepository;
        }

        public ICollection<FoodItemViewModel> GetAllDrinksInOrderAsFoodItemViewModel(string orderId)
        {
            return this.orderDrinkRepository
                .AllAsNoTracking()
                .Where(x => x.OrderId == orderId)
                .To<FoodItemViewModel>()
                .ToList();
        }

        public ICollection<OrderDeliveredItemDto> GetDrinksAsOrderDeliveredItemById(string orderId)
        {
            return this.orderDrinkRepository
                .All()
                .Where(x => x.OrderId == orderId)
                .Select(x => new OrderDeliveredItemDto()
                {
                    Count = x.Count,
                    DeliveredCount = x.DeliveredCount,
                })
                .ToArray();
        }

        public async Task AddDeliveredCountToOrderDrinkAsync(string orderId, string foodId, int count)
        {
            var orderDrinkItem = this.orderDrinkRepository
                .All()
                .FirstOrDefault(x => x.OrderId == orderId && x.DrinkId == foodId);

            if (orderDrinkItem.Count - orderDrinkItem.DeliveredCount <= 0)
            {
                throw new InvalidOperationException("The item has already been made!");
            }

            orderDrinkItem.DeliveredCount += count;
            await this.orderDrinkRepository.SaveChangesAsync();
        }

        public ICollection<CookItemViewModel> GetAllCookingDrinksInOrder(string orderId)
        {
            return this.orderDrinkRepository.All()
                .Where(x => x.Order.ProcessType == ProcessType.Cooking && x.Count - x.DeliveredCount > 0 && (orderId == null || x.OrderId == orderId))
                .OrderBy(x => x.Order.CreatedOn)
                .Select(x => new CookItemViewModel()
                {
                    Count = x.Count - x.DeliveredCount,
                    FoodId = x.DrinkId,
                    FoodName = x.Drink.Name,
                    OrderId = x.OrderId,
                }).ToList();
        }

        public ICollection<AnalyseCountViewModel> GetDrinksForWaiterAnalyse(string waiterId, DateTime startDate)
        {
            return this.orderDrinkRepository
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

        public PickupItem GetOrderDrinkAsPickupItem(string foodId, string orderId)
        {
            return this.orderDrinkRepository
                .All()
                .Where(x => x.OrderId == orderId && x.DrinkId == foodId)
                .Select(x => new PickupItem()
                {
                    ClientName = x.Order.Client.FirstName + " " + x.Order.Client.LastName,
                    Name = x.Drink.Name,
                    TableNumber = x.Order.Table.Number,
                    WaiterId = x.Order.WaiterId,
                    Count = 1,
                    OrderId = orderId,
                })
                .FirstOrDefault();
        }

        public bool AreAllDrinksDelivered(string orderId)
        {
            return this.orderDrinkRepository
                    .All()
                    .Where(x => x.OrderId == orderId)
                    .All(x => x.DeliveredCount >= x.Count);
        }

        public ICollection<SalesChartViewModel> GetDailyDrinkIncomeByPeriod(DateTime startDate, DateTime endDate)
        {
            return this.orderDrinkRepository
                    .All()
                    .Where(x => x.Order.DeliveredOn.Value.Date >= startDate.Date && x.Order.DeliveredOn.Value.Date <= endDate.Date)
                    .GroupBy(x => new { x.Order.DeliveredOn.Value.Day, x.Order.DeliveredOn.Value.Month, x.Order.DeliveredOn.Value.Year })
                    .Select(x => new SalesChartViewModel()
                    {
                        Date = new DateTime(x.Key.Year, x.Key.Month, x.Key.Day).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture),
                        Income = x.Sum(y => y.PriceForOne * y.Count),
                    }).ToList();
        }

        public ICollection<SalesChartViewModel> GetMonthlyDrinkIncomeByPeriod(DateTime startDate, DateTime endDate)
        {
            return this.orderDrinkRepository
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

        public ICollection<SalesChartViewModel> GetYearlyDrinkIncomeByPeriod(DateTime startDate, DateTime endDate)
        {
            return this.orderDrinkRepository
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
