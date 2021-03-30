namespace RestaurantMenuProject.Web.ViewModels
{
    using AutoMapper;
    using RestaurantMenuProject.Data.Models;
    using RestaurantMenuProject.Data.Models.Enums;
    using RestaurantMenuProject.Services.Mapping;

    public class ActiveOrderViewModel : IMapFrom<Order>, IHaveCustomMappings
    {
        public string Id { get; set; }

        public ProcessType ProcessType { get; set; }

        public int TableNumber { get; set; }

        public bool IsPaid { get; set; }

        [IgnoreMap]
        public double ReadyPercent { get; set; }

        public void CreateMappings(IProfileExpression configuration)
        {
            configuration.CreateMap<Order, ActiveOrderViewModel>()
                .ForMember(x => x.IsPaid, y => y.MapFrom(x => x.PaidOn != null));
        }
    }
}
