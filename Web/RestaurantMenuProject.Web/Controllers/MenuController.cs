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

        public IActionResult DisplayFood(string type, int id)
        {
            if (id == 0)
            {
                var dishes = this.dishTypeService.GetAllDisheshWithDishType(type);
                return this.View("DisplayFoodType", dishes);
            }
            else
            {
                var dish = this.dishTypeService.GetDishWithId(id);
                return this.View("DisplayFood", dish);
            }
        }

        //[Route("Menu/Drinks/{type}/{id?}")]
        public IActionResult DisplayDrink(string type, int id)
        {
            if (id == 0)
            {
                var drinks = this.drinkTypeService.GetAllDrinksByType(type);
                return this.View("DisplayDrinkType", drinks);
            }
            else
            {
                var drink = this.drinkTypeService.GetDrinkById(id);
                return this.View("DisplayDrink", drink);
            }
        }

        public IActionResult Index()
        {
            return this.View(this.dishTypeService.GetAllDishTypes());
        }

        public IActionResult Drinks()
        {
            return this.View(this.drinkTypeService.GetAllDrinkTypes());
        }

        public IActionResult Drink(int id)
        {
            // GET THE DRINK AND DISPLAY IT, PASS IT TO THE VIEW
            return this.View();
        }
    }
}
