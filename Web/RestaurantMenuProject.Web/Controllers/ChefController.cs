using Microsoft.AspNetCore.Mvc;

namespace RestaurantMenuProject.Web.Controllers
{
    public class ChefController : Controller
    {
        public IActionResult Index()
        {
            return this.View();
        }
    }
}
