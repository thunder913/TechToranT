namespace RestaurantMenuProject.Web.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using RestaurantMenuProject.Services.Data.Contracts;

    public class MenuController : Controller
    {
        private readonly IDrinkTypeService drinkTypeService;
        private readonly IDishTypeService dishTypeService;
        private readonly IDishService dishService;

        public MenuController(IDrinkTypeService drinkTypeService, IDishTypeService dishTypeService, IDishService dishService)
        {
            this.drinkTypeService = drinkTypeService;
            this.dishTypeService = dishTypeService;
            this.dishService = dishService;
        }

        public IActionResult DisplayFood(string type, int id)
        {
            if (id == 0)
            {
                var dishes = this.dishService.GetAllDisheshWithDishTypeAsFoodItem(type);
                return this.View("DisplayFoodType", dishes);
            }
            else
            {
                var dish = this.dishService.GetDishAsFoodItemById(id);
                return this.View("DisplayFood", dish);
            }
        }

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
    }
}
