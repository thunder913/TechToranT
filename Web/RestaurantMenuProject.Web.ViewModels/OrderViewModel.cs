namespace RestaurantMenuProject.Web.ViewModels
{
    using System;
    using System.Globalization;
    using System.Linq;

    using AutoMapper;
    using RestaurantMenuProject.Data.Models;
    using RestaurantMenuProject.Data.Models.Enums;
    using RestaurantMenuProject.Services.Mapping;

    public class OrderViewModel : IMapFrom<Order>, IHaveCustomMappings
    {
        public string Id { get; set; }

        public DateTime Date { get; set; }

        public ProcessType Status { get; set; }

        public decimal Price { get; set; }

        public void CreateMappings(IProfileExpression configuration)
        {
            configuration.CreateMap<Order, OrderViewModel>()
                .ForMember(x => x.Date, y => y.MapFrom(x => x.CreatedOn))
                .ForMember(x => x.Status, y => y.MapFrom(x => x.ProcessType))
                .ForMember(x => x.Price, y =>
                y.MapFrom(x => x.OrderDishes.Sum(d => d.Dish.Price * d.Count) + x.OrderDrinks.Sum(d => d.Drink.Price * d.Count)));
        }
    }
}
