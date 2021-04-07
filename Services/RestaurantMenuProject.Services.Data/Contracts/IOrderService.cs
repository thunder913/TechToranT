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
        public Task<string> MakeOrderAsync(string userId, string tableCode);

        public Task<bool> CancelOrderAsync(string orderId);

        public int GetUserOrdersCount(string userId);

        public ICollection<FoodItemViewModel> GetAllFoodItemsById(string orderId);

        public OrderInfoViewModel GetFullInformationForOrder(string orderId);

        public ICollection<ManageOrderViewModel> GetAllOrders(string sortColumn, string sortDirection, string searchValue);

        public Task ChangeOrderStatusAsync(ProcessType oldProcessType, ProcessType newProcessType, string orderId);

        public ICollection<OrderInListViewModel> GetOrdersWithStatus(ProcessType processType);

        public Task AddWaiterToOrderAsync(string orderId, string waiterId);

        public ICollection<ActiveOrderViewModel> GetActiveOrders(string waiterId);

        public WaiterViewModel GetWaiterViewModel(string userId);

        public Task FinishOrderAsync(string orderId);

        public ChefViewModel GetChefViewModel();

        public ICollection<CookFoodCategoriesViewModel> GetCookFoodTypes(string id);

        public Task AddDeliveredCountToOrderDrinkAsync(int count, CookFinishItemViewModel itemViewModel);

        public Task AddDeliveredCountToOrderDishAsync(int count, CookFinishItemViewModel itemViewModel);

        public PickupItem GetOrderDishAsPickupItem(CookFinishItemViewModel itemViewModel);

        public PickupItem GetOrderDrinkAsPickupItem(CookFinishItemViewModel itemViewModel);

        public double GetOrderDeliveredPerCent(string orderId);

        public SalesViewModel GetSalesDataForPeriod(DateTime startDate, DateTime endDate, string period);

        public Task<ICollection<StaffAnalyseViewModel>> GetAllStaffForAnalyseAsync(DateTime startDate);

        public OrderInListViewModel GetOrderInListById(string id);

        public string GetWaiterId(string id);

        public ActiveOrderViewModel GetActiveOrderById(string id);

        public bool IsOrderCooked(string orderId);

        public bool IsOrderPaid(string orderId);

        public Task PayOrderByIdAsync(string id);
    }
}
