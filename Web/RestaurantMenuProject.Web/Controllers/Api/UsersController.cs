namespace RestaurantMenuProject.Web.Controllers.Api
{
    using System;
    using System.Linq;
    using System.Linq.Dynamic.Core;

    using Microsoft.AspNetCore.Mvc;
    using RestaurantMenuProject.Data;
    using RestaurantMenuProject.Services.Data.Contracts;

    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService userService;

        public UsersController(IUserService userService)
        {
            this.userService = userService;
        }

        [HttpPost]
        public ActionResult Index()
        {
                var draw = this.Request.Form["draw"].FirstOrDefault();
                var start = this.Request.Form["start"].FirstOrDefault();
                var length = this.Request.Form["length"].FirstOrDefault();
                var sortColumn = this.Request.Form["columns[" + this.Request.Form["order[0][column]"].FirstOrDefault() + "][name]"].FirstOrDefault();
                var sortColumnDirection = this.Request.Form["order[0][dir]"].FirstOrDefault();
                var searchValue = this.Request.Form["search[value]"].FirstOrDefault();
                int pageSize = length != null ? Convert.ToInt32(length) : 0;
                int skip = start != null ? Convert.ToInt32(start) : 0;

                var userData = this.userService.GetUserDataAsQueryable(sortColumn, sortColumnDirection, searchValue);

                var recordsTotal = userData.Count();
                var data = userData.Skip(skip).Take(pageSize).ToList();
                var jsonData = new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data };

                return this.Ok(jsonData);
        }
    }
}
