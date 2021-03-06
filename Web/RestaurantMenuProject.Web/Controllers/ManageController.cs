using Microsoft.AspNetCore.Mvc;

namespace RestaurantMenuProject.Web.Controllers
{
    public class ManageController : Controller
    {
        public IActionResult Index()
        {
            return this.View();
        }

        [HttpPost]
        public IActionResult Add(string type)
        {
            return this.View();
        }

        public IActionResult Remove()
        {
            return this.View();
        }
    }
}
