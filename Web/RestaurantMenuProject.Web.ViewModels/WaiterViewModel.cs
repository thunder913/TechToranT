namespace RestaurantMenuProject.Web.ViewModels
{
    using System.Collections.Generic;
    using RestaurantMenuProject.Data.Models;
    using RestaurantMenuProject.Data.Models.Dtos;

    public class WaiterViewModel
    {
        public ICollection<OrderInListViewModel> NewOrders { get; set; } = new HashSet<OrderInListViewModel>();

        public ICollection<PickupItem> PickupItems { get; set; } = new HashSet<PickupItem>();

        public ICollection<ActiveOrderViewModel> ActiveOrders { get; set; } = new HashSet<ActiveOrderViewModel>();
    }
}
