namespace RestaurantMenuProject.Web.Controllers.Api
{
    using Microsoft.AspNetCore.Mvc;
    using RestaurantMenuProject.Data.Models.Dtos;
    using RestaurantMenuProject.Services.Data.Contracts;

    [Route("api/[Controller]")]
    [ApiController]
    public class OrderController : BaseController
    {
        private readonly IOrderService orderService;

        public OrderController(
            IOrderService orderService)
        {
            this.orderService = orderService;
        }

        [HttpPost("Delete")]
        public ActionResult<bool> Delete(OrderIdDto orderDto)
        {
            return this.orderService.DeleteById(orderDto.OrderId).Result;
        }
    }
}
