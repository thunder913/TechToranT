namespace RestaurantMenuProject.Services.Data.Contracts
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using RestaurantMenuProject.Web.ViewModels;

    public interface IOrderService
    {
        public ICollection<OrderInListViewModel> GetOrderViewModelsByUserId(string userId, int page, int itemsPerPage);

        public Task MakeOrder(string userId);

        public Task<bool> DeleteById(string orderId);

        public int GetUserOrdersCount(string userId);

        public ICollection<FoodItemViewModel> GetAllDishesInOrder(string orderId);

        public OrderInfoViewModel GetFullInformationForOrder(string orderId);
    }
}
