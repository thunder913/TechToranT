using Microsoft.AspNetCore.Mvc;
using RestaurantMenuProject.Services.Data.Contracts;
using System;
using System.Linq;

namespace RestaurantMenuProject.Web.Controllers.Api
{
    [Route("api/[Controller]")]
    [ApiController]
    public class PromoCodeController : ControllerBase
    {
        private readonly IPromoCodeService promoCodeService;

        public PromoCodeController(IPromoCodeService promoCodeService)
        {
            this.promoCodeService = promoCodeService;
        }

        [HttpPost("All")]
        public ActionResult GetAllPromoCodes()
        {
            var draw = this.Request.Form["draw"].FirstOrDefault();
            var start = this.Request.Form["start"].FirstOrDefault();
            var length = this.Request.Form["length"].FirstOrDefault();
            var sortColumn = this.Request.Form["columns[" + this.Request.Form["order[0][column]"].FirstOrDefault() + "][name]"].FirstOrDefault();
            var sortColumnDirection = this.Request.Form["order[0][dir]"].FirstOrDefault();
            var searchValue = this.Request.Form["search[value]"].FirstOrDefault();
            int pageSize = length != null ? Convert.ToInt32(length) : 0;
            int skip = start != null ? Convert.ToInt32(start) : 0;

            var orders = this.promoCodeService.GetAllPromoCodes(sortColumn, sortColumnDirection, searchValue);

            var recordsTotal = orders.Count();
            var data = orders.Skip(skip).Take(pageSize).ToList();
            var jsonData = new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data };

            return this.Ok(jsonData);
        }

    }
}
