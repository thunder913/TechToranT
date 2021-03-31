using Microsoft.AspNetCore.Mvc;
using RestaurantMenuProject.Services.Data.Contracts;
using System;

namespace RestaurantMenuProject.Web.Controllers
{
    public class AnalyseController : Controller
    {
        private readonly IOrderService orderService;

        public AnalyseController(IOrderService orderService)
        {
            this.orderService = orderService;
        }

        public IActionResult Sales()
        {
            var oneYearAgo = DateTime.UtcNow;
            oneYearAgo = oneYearAgo.AddDays(-6);
            var sales = this.orderService.GetSalesDataForPeriod(oneYearAgo, DateTime.UtcNow, "daily");
            sales.Type = "Daily";
            return this.View(sales);
        }
    }
}
