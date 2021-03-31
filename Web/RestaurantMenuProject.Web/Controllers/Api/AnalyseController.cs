using Microsoft.AspNetCore.Mvc;
using RestaurantMenuProject.Services.Data.Contracts;
using RestaurantMenuProject.Web.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RestaurantMenuProject.Web.Controllers.Api
{
    [Route("api/[Controller]")]
    [ApiController]
    public class AnalyseController : ControllerBase
    {
        private readonly IOrderService orderService;

        public AnalyseController(IOrderService orderService)
        {
            this.orderService = orderService;
        }

        [HttpGet("Incomes")]
        public ActionResult<SalesViewModel> GetIncomes([FromQuery]GetIncomesViewModel incomesViewModel)
        {
            var data = this.orderService.GetSalesDataForPeriod(DateTime.Parse(incomesViewModel.StartDate), DateTime.Parse(incomesViewModel.EndDate), incomesViewModel.Period);
            data.Type = incomesViewModel.Period;
            return data;
        }

    }
}
