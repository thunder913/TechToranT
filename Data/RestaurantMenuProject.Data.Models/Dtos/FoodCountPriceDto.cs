using AutoMapper;
using RestaurantMenuProject.Services.Mapping;
using System.Collections.Generic;
using System.Linq;

namespace RestaurantMenuProject.Data.Models.Dtos
{
    public class FoodCountPriceDto : IMapFrom<BasketDish>, IMapFrom<BasketDrink>, IMapTo<OrderDish>, IMapTo<OrderDrink>, IHaveCustomMappings
    {
        public string Id { get; set; }

        public int Quantity { get; set; }

        public decimal Price { get; set; }

        public string CategoryName { get; set; }

        public void CreateMappings(IProfileExpression configuration)
        {
            configuration.CreateMap<ICollection<BasketDish>, ICollection<FoodCountPriceDto>>()
                .ForMember(x => x, y => y.MapFrom(x => x.Select(d => new FoodCountPriceDto() { Id = d.DishId, Quantity = d.Quantity})));

            configuration.CreateMap<ICollection<BasketDrink>, ICollection<FoodCountPriceDto>>()
                .ForMember(x => x, y => y.MapFrom(x => x.Select(d => new FoodCountPriceDto() { Id = d.DrinkId, Quantity = d.Quantity })));

            configuration.CreateMap<BasketDish, FoodCountPriceDto>()
                .ForMember(x => x.Id, y => y.MapFrom(x => x.DishId));

            configuration.CreateMap<BasketDrink, FoodCountPriceDto>()
                .ForMember(x => x.Id, y => y.MapFrom(x => x.DrinkId));

            configuration.CreateMap<FoodCountPriceDto, OrderDish>()
                .ForMember(x => x.DishId, y => y.MapFrom(x => x.Id))
                .ForMember(x => x.Count, y => y.MapFrom(x => x.Quantity));

            configuration.CreateMap<FoodCountPriceDto, OrderDrink>()
                .ForMember(x => x.DrinkId, y => y.MapFrom(x => x.Id))
                .ForMember(x => x.Count, y => y.MapFrom(x => x.Quantity));
        }
    }
}
