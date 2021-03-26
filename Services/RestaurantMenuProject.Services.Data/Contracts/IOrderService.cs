namespace RestaurantMenuProject.Services.Data.Contracts
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
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
    }
}
