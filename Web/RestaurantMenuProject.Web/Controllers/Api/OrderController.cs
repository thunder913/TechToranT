namespace RestaurantMenuProject.Web.Controllers.Api
{
    using Microsoft.AspNetCore.Mvc;
    using RestaurantMenuProject.Data.Models.Dtos;

    [IgnoreAntiforgeryToken]
    [Route("api/[Controller]")]
    [ApiController]
    public class OrderController : BaseController
    {
        // TODO make order actions works
        [HttpPost("Delete")]
        public string Delete(string orderId)
        {
            return orderId;
        }
    }
}
