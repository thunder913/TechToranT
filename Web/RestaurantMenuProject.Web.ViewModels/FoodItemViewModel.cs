namespace RestaurantMenuProject.Web.ViewModels
{
    using AutoMapper;
    using RestaurantMenuProject.Data.Models;
    using RestaurantMenuProject.Data.Models.Enums;
    using RestaurantMenuProject.Services.Mapping;

    public class FoodItemViewModel : IMapFrom<Dish>, IMapFrom<Drink>, IHaveCustomMappings
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
            configuration.CreateMap<Dish, FoodItemViewModel>()
                .ForMember(x => x.Id, y => y.MapFrom(x => x.Id))
                .ForMember(x => x.Name, y => y.MapFrom(x => x.Name))
                .ForMember(x => x.FoodType, y => y.MapFrom(x => FoodType.Dish))
                .ForMember(x => x.FoodCategory, y => y.MapFrom(x => x.DishType.Name))
                .ForMember(x => x.Image, y => y.MapFrom(x => x.Image));

            configuration.CreateMap<Drink, FoodItemViewModel>()
                .ForMember(x => x.Id, y => y.MapFrom(x => x.Id))
                .ForMember(x => x.Name, y => y.MapFrom(x => x.Name))
                .ForMember(x => x.FoodType, y => y.MapFrom(x => FoodType.Drink))
                .ForMember(x => x.FoodCategory, y => y.MapFrom(x => x.DrinkType.Name))
                .ForMember(x => x.Image, y => y.MapFrom(x => x.Image));
        }
    }
}
