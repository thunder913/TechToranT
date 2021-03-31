namespace RestaurantMenuProject.Web.ViewModels
{
    using System.Collections.Generic;

    public class SalesViewModel
    {
        public ICollection<SalesChartViewModel> TotalIncome { get; set; } = new HashSet<SalesChartViewModel>();

        public ICollection<SalesChartViewModel> DishIncome { get; set; } = new HashSet<SalesChartViewModel>();

        public ICollection<SalesChartViewModel> DrinkIncome { get; set; } = new HashSet<SalesChartViewModel>();

        public string Type { get; set; }
    }
}
