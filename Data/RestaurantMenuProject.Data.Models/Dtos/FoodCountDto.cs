using AutoMapper;
using RestaurantMenuProject.Services.Mapping;
using System.Collections.Generic;
using System.Linq;

namespace RestaurantMenuProject.Data.Models.Dtos
{
    public class FoodCountDto : IMapFrom<BasketDish>, IMapFrom<BasketDrink>, IMapTo<OrderDish>, IMapTo<OrderDrink>, IHaveCustomMappings
    {
        public string Id { get; set; }

        public int Quantity { get; set; }

        public void CreateMappings(IProfileExpression configuration)
        {
            configuration.CreateMap<ICollection<BasketDish>, ICollection<FoodCountDto>>()
                .ForMember(x => x, y => y.MapFrom(x => x.Select(d => new FoodCountDto() { Id = d.DishId, Quantity = d.Quantity})));

            configuration.CreateMap<ICollection<BasketDrink>, ICollection<FoodCountDto>>()
                .ForMember(x => x, y => y.MapFrom(x => x.Select(d => new FoodCountDto() { Id = d.DrinkId, Quantity = d.Quantity })));

            configuration.CreateMap<BasketDish, FoodCountDto>()
                .ForMember(x => x.Id, y => y.MapFrom(x => x.DishId));

            configuration.CreateMap<BasketDrink, FoodCountDto>()
                .ForMember(x => x.Id, y => y.MapFrom(x => x.DrinkId));

            configuration.CreateMap<FoodCountDto, OrderDish>()
                .ForMember(x => x.DishId, y => y.MapFrom(x => x.Id))
                .ForMember(x => x.Count, y => y.MapFrom(x => x.Quantity));

            configuration.CreateMap<FoodCountDto, OrderDrink>()
                .ForMember(x => x.DrinkId, y => y.MapFrom(x => x.Id))
                .ForMember(x => x.Count, y => y.MapFrom(x => x.Quantity));
        }
    }
}
