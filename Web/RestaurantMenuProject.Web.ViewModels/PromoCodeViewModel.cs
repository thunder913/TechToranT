using AutoMapper;
using RestaurantMenuProject.Data.Models;
using RestaurantMenuProject.Services.Mapping;
using System;

namespace RestaurantMenuProject.Web.ViewModels
{
    public class PromoCodeViewModel : IMapFrom<PromoCode>
    {
        public int Id { get; set; }

        public string Code { get; set; }

        public int MaxUsageTimes { get; set; }

        public DateTime ExpirationDate { get; set; }

        public int UsedTimes { get; set; }

        public int PromoPercent { get; set; }

        public bool IsDeleted { get; set; }
    }
}
