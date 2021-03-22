using AutoMapper;
using RestaurantMenuProject.Data.Models.Enums;
using RestaurantMenuProject.Services.Mapping;
using System;

namespace RestaurantMenuProject.Web.ViewModels
{
    public class ManageOrderViewModel : IMapFrom<OrderInListViewModel>, IHaveCustomMappings
    {
        public string Id { get; set; }

        public DateTime Date { get; set; }

        public string Status { get; set; }

        public decimal Price { get; set; }

        public string FullName { get; set; }

        public string Email { get; set; }

        public void CreateMappings(IProfileExpression configuration)
        {
            configuration.CreateMap<OrderInListViewModel, ManageOrderViewModel>()
                .ForMember(x => x.Status, y => y.MapFrom(x => x.Status.ToString()));
        }
    }
}
