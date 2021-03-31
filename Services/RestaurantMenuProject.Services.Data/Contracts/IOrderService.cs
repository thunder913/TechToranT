namespace RestaurantMenuProject.Services.Data.Contracts
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using RestaurantMenuProject.Data.Models;
    using RestaurantMenuProject.Data.Models.Enums;
    using RestaurantMenuProject.Web.ViewModels;

    public interface IOrderService
    {
        public ICollection<OrderInListViewModel> GetOrderViewModelsByUserId(int page, int itemsPerPage, string userId = null);
        public Task MakeOrder(string userId, string tableCode);

        public Task<bool> CancelOrder(string orderId);

        public int GetUserOrdersCount(string userId);

        public ICollection<FoodItemViewModel> GetAllFoodItemsById(string orderId);

        public OrderInfoViewModel GetFullInformationForOrder(string orderId);

        public ICollection<ManageOrderViewModel> GetAllOrders(string sortColumn, string sortDirection, string searchValue);

        public void ChangeOrderStatus(ProcessType oldProcessType, ProcessType newProcessType, string orderId);

        public ICollection<OrderInListViewModel> GetOrdersWithStatus(ProcessType processType);

        public void AddWaiterToOrder(string orderId, string waiterId);

        public ICollection<ActiveOrderViewModel> GetActiveOrders(string waiterId);

        public WaiterViewModel GetWaiterViewModel(string userId);

        public Task FinishOrder(string orderId);

        public ChefViewModel GetChefViewModel();

        public ICollection<CookFoodCategoriesViewModel> GetCookFoodTypes();

        public Task AddDeliveredCountToOrderDrink(int count, CookFinishItemViewModel itemViewModel);

        public Task AddDeliveredCountToOrderDish(int count, CookFinishItemViewModel itemViewModel);

        public PickupItem GetOrderDishAsPickupItem(CookFinishItemViewModel itemViewModel);

        public PickupItem GetOrderDrinkAsPickupItem(CookFinishItemViewModel itemViewModel);

        public double GetOrderDeliveredPerCent(string orderId);

        public SalesViewModel GetSalesDataForPeriod(DateTime startDate, DateTime endDate, string period);
    }
}
