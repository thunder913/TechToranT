using Microsoft.AspNetCore.Mvc;
using RestaurantMenuProject.Data.Models.Enums;
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
    public class ChefController : ControllerBase
    {
        private readonly IPickupItemService pickupItemService;
        private readonly IOrderService orderService;

        public ChefController(
            IPickupItemService pickupItemService,
            IOrderService orderService
            )
        {
            this.pickupItemService = pickupItemService;
            this.orderService = orderService;
        }

        [HttpPost("AddItem")]
        public async Task<ActionResult<bool>> FinishOne(CookFinishItemViewModel foodItem)
        {
            foodItem.DishType = (FoodType) Enum.Parse(typeof(FoodType), foodItem.FoodType);
            return await this.pickupItemService.AddPickupItemAsync(foodItem);
        }

        [HttpPost("AcceptOrder/{id}")]
        public ActionResult<bool> AcceptOrder(string id)
        {
            this.orderService.ChangeOrderStatus(ProcessType.InProcess, ProcessType.Cooking, id);
            return true;
        }
    }
}
