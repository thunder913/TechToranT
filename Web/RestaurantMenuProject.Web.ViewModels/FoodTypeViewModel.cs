namespace RestaurantMenuProject.Web.ViewModels
{
    using RestaurantMenuProject.Data.Models;
    using RestaurantMenuProject.Services.Mapping;

    public class FoodTypeViewModel : IMapFrom<DishType>, IMapFrom<DrinkType>
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }
    }
}
