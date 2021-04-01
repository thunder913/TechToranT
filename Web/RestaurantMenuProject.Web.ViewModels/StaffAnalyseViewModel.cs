using System.Collections.Generic;

namespace RestaurantMenuProject.Web.ViewModels
{
    public class StaffAnalyseViewModel
    {
        public ICollection<StaffAnalyseOrdersViewModel> OrdersData { get; set; } = new HashSet<StaffAnalyseOrdersViewModel>();

        public string FullName { get; set; }
    }
}
