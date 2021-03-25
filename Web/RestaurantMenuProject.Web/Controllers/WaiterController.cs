namespace RestaurantMenuProject.Web.Controllers
{
    using Microsoft.AspNetCore.Mvc;

    public class WaiterController : Controller
    {
        public IActionResult Index()
        {
            return this.View();
        }
    }
}
