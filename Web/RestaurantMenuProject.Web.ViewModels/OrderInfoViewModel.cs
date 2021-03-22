namespace RestaurantMenuProject.Web.ViewModels
{
    using System;
    using System.Collections.Generic;

    using AutoMapper;
    using RestaurantMenuProject.Data.Models;
    using RestaurantMenuProject.Data.Models.Enums;
    using RestaurantMenuProject.Services.Mapping;

    public class OrderInfoViewModel : IMapFrom<Order>
    {
        public string Id { get; set; }

        public ProcessType ProcessType { get; set; }

        public DateTime? PaidOn { get; set; }

        public PromoCode PromoCode { get; set; }

        public DateTime CreatedOn { get; set; }

        [IgnoreMap]
        public ICollection<FoodItemViewModel> FoodItems { get; set; } = new HashSet<FoodItemViewModel>();

        public string ClientId { get; set; }
    }
}
