namespace RestaurantMenuProject.Web.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using RestaurantMenuProject.Services.Data.Contracts;

    public class MenuController : Controller
    {
        private readonly IDrinkTypeService drinkTypeService;
        private readonly IDishTypeService dishTypeService;

        public MenuController(IDrinkTypeService drinkTypeService, IDishTypeService dishTypeService)
        {
            this.drinkTypeService = drinkTypeService;
            this.dishTypeService = dishTypeService;
        }

        public IActionResult Index()
        {
            return this.View(this.dishTypeService.GetAllDishTypes());
        }

        public IActionResult Drinks()
        {
            return this.View(this.drinkTypeService.GetAllDrinkTypes());
        }
    }
}
