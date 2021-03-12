namespace RestaurantMenuProject.Web.Controllers
{
    using System.Diagnostics;

    using Microsoft.AspNetCore.Mvc;
    using RestaurantMenuProject.Services.Data.Contracts;
    using RestaurantMenuProject.Web.ViewModels;

    public class HomeController : BaseController
    {
        private readonly IDishTypeService dishTypeService;
        private readonly IDrinkTypeService drinkTypeService;

        public HomeController(IDishTypeService dishTypeService, IDrinkTypeService drinkTypeService)
        {
            this.dishTypeService = dishTypeService;
            this.drinkTypeService = drinkTypeService;
        }

        public IActionResult Index()
        {
            return this.View();
        }

        public IActionResult Privacy()
        {
            return this.View();
        }

        public IActionResult Add()
        {
            return this.View();
        }

        public IActionResult Remove()
        {
            return this.View();
        }

        public IActionResult Menu()
        {
            return this.View(this.dishTypeService.GetAllDishTypes());
        }

        public IActionResult Manage()
        {
            return this.View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return this.View(
                new ErrorViewModel { RequestId = Activity.Current?.Id ?? this.HttpContext.TraceIdentifier });
        }
    }
}
