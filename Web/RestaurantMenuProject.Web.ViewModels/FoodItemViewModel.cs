namespace RestaurantMenuProject.Web.ViewModels
{
    using AutoMapper;
    using RestaurantMenuProject.Data.Models;
    using RestaurantMenuProject.Data.Models.Enums;
    using RestaurantMenuProject.Services.Mapping;

    public class FoodItemViewModel : IMapFrom<OrderDish>, IMapFrom<OrderDrink>, IHaveCustomMappings
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public int Quantity { get; set; }

        public decimal Price { get; set; }

        public FoodType FoodType { get; set; }

        public string FoodCategory { get; set; }

        public Image Image { get; set; }

        public void CreateMappings(IProfileExpression configuration)
        {
            configuration.CreateMap<OrderDish, FoodItemViewModel>()
                .ForMember(x => x.Id, y => y.MapFrom(x => x.Dish.Id))
                .ForMember(x => x.Name, y => y.MapFrom(x => x.Dish.Name))
                .ForMember(x => x.Quantity, y => y.MapFrom(x => x.Count))
                .ForMember(x => x.Price, y => y.MapFrom(x => x.Dish.Price))
                .ForMember(x => x.FoodType, y => y.MapFrom(x => FoodType.Dish))
                .ForMember(x => x.FoodCategory, y => y.MapFrom(x => x.Dish.DishType.Name))
                .ForMember(x => x.Image, y => y.MapFrom(x => x.Dish.Image));

            configuration.CreateMap<OrderDrink, FoodItemViewModel>()
                .ForMember(x => x.Id, y => y.MapFrom(x => x.Drink.Id))
                .ForMember(x => x.Name, y => y.MapFrom(x => x.Drink.Name))
                .ForMember(x => x.Quantity, y => y.MapFrom(x => x.Count))
                .ForMember(x => x.Price, y => y.MapFrom(x => x.Drink.Price))
                .ForMember(x => x.FoodType, y => y.MapFrom(x => FoodType.Drink))
                .ForMember(x => x.FoodCategory, y => y.MapFrom(x => x.Drink.DrinkType.Name))
                .ForMember(x => x.Image, y => y.MapFrom(x => x.Drink.Image));
        }
    }
}
