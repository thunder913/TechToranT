using RestaurantMenuProject.Data.Models;
using RestaurantMenuProject.Services.Mapping;

namespace RestaurantMenuProject.Web.ViewModels
{
    public class MenuItemViewModel : IMapFrom<DishType>, IMapFrom<DrinkType>
    {
        public string Name { get; set; }

        public string Description { get; set; }
    }
}
