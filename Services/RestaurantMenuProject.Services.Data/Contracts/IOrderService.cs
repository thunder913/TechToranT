namespace RestaurantMenuProject.Services.Data.Contracts
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using RestaurantMenuProject.Web.ViewModels;

    public interface IOrderService
    {
        public ICollection<OrderViewModel> GetOrderViewModelsByUserId(string userId);

        public Task MakeOrder(string userId);
    }
}
