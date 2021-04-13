namespace RestaurantMenuProject.Web.Controllers.Api
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Mvc;
    using RestaurantMenuProject.Services.Data.Contracts;

    [Route("api/[Controller]")]
    [ApiController]
    public class TableController : ControllerBase
    {
        private readonly ITableService tableService;

        public TableController(ITableService tableService)
        {
            this.tableService = tableService;
        }

        [HttpGet("GetNextNumber")]
        public int GetNextNumber()
        {
            return this.tableService.GetFreeTable();
        }
    }
}
