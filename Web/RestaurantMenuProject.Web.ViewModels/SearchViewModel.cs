namespace RestaurantMenuProject.Web.ViewModels
{
    using System.Collections.Generic;

    public class SearchViewModel
    {
        public string SearchTerm { get; set; }

        public ICollection<DrinkItemViewModel> Drinks { get; set; } = new HashSet<DrinkItemViewModel>();

        public ICollection<DishViewModel> Dishes { get; set; } = new HashSet<DishViewModel>();
    }
}
