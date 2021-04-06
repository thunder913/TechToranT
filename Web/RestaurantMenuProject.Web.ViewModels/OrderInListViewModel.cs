namespace RestaurantMenuProject.Web.ViewModels
{
    using System;
    using System.Globalization;
    using System.Linq;

    using AutoMapper;
    using RestaurantMenuProject.Data.Models;
    using RestaurantMenuProject.Data.Models.Enums;
    using RestaurantMenuProject.Services.Mapping;

    public class OrderInListViewModel : IMapFrom<Order>, IHaveCustomMappings
    {
        public string Id { get; set; }

        public DateTime Date { get; set; }

        public ProcessType Status { get; set; }

        public string StatusName { get; set; }

        public decimal Price { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string FullName { get; set; }

        public string Email { get; set; }

        public int TableNumber { get; set; }

        public void CreateMappings(IProfileExpression configuration)
        {
            configuration.CreateMap<Order, OrderInListViewModel>()
                .ForMember(x => x.Date, y => y.MapFrom(x => x.CreatedOn))
                .ForMember(x => x.Status, y => y.MapFrom(x => x.ProcessType))
                .ForMember(x => x.Price, y =>
                y.MapFrom(x => x.OrderDishes.Sum(d => d.PriceForOne * d.Count) + x.OrderDrinks.Sum(d => d.PriceForOne * d.Count)))
                .ForMember(x => x.FirstName, y => y.MapFrom(x => x.Client.FirstName))
                .ForMember(x => x.LastName, y => y.MapFrom(x => x.Client.LastName))
                .ForMember(x => x.Email, y => y.MapFrom(x => x.Client.Email))
                .ForMember(x => x.FullName, y => y.MapFrom(x => x.Client.FirstName + " " + x.Client.LastName));
        }
    }
}
