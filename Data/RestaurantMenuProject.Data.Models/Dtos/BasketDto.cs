using AutoMapper;
using RestaurantMenuProject.Services.Mapping;
using System.Collections.Generic;
using System.Linq;

namespace RestaurantMenuProject.Data.Models.Dtos
{
    public class BasketDto : IMapFrom<Basket>, IMapTo<Order>, IHaveCustomMappings
    {
        public string Id { get; set; }

        public ICollection<FoodCountDto> Dishes { get; set; } = new HashSet<FoodCountDto>();

        public ICollection<FoodCountDto> Drinks { get; set; } = new HashSet<FoodCountDto>();

        public void CreateMappings(IProfileExpression configuration)
        {
            configuration.CreateMap<Basket, BasketDto>()
                .ForMember(x => x.Dishes, y => y.MapFrom(x => x.Dishes))
                .ForMember(x => x.Drinks, y => y.MapFrom(x => x.Drinks));

            configuration.CreateMap<BasketDto, Order>()
                .ForMember(x => x.ClientId, y => y.MapFrom(x => x.Id))
                .ForMember(x => x.OrderDishes, y => y.MapFrom(x => x.Dishes.Select(d => new OrderDish()
                {
                    Count = d.Quantity,
                    DishId = d.Id,
                }).ToList()))
                .ForMember(x => x.OrderDrinks, y => y.MapFrom(x => x.Dishes.Select(d => new OrderDrink()
                {
                    Count = d.Quantity,
                    DrinkId = d.Id,
                }).ToList()));
        }
    }
}
