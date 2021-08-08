using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestaurantMenuProject.Services.Data.Contracts;
using System;
using System.Threading.Tasks;

namespace RestaurantMenuProject.Web.Controllers
{
    [Authorize]
    public class AnalyseController : Controller
    {
        private readonly IOrderService orderService;

        public AnalyseController(IOrderService orderService)
        {
            this.orderService = orderService;
        }

        [Authorize]
        public IActionResult Sales()
        {
            var oneYearAgo = DateTime.UtcNow;
            oneYearAgo = oneYearAgo.AddDays(-6);
            var sales = this.orderService.GetSalesDataForPeriod(oneYearAgo, DateTime.UtcNow, "daily");
            sales.Type = "Daily";
            return this.View(sales);
        }

        [Authorize]
        public async Task<IActionResult> Staff()
        {
            var date = DateTime.UtcNow;
            date = date.AddYears(-1);
            var staff = await this.orderService.GetAllStaffForAnalyseAsync(date);
            return this.View(staff);
        }
    }
}
