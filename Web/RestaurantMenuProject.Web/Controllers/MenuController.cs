namespace RestaurantMenuProject.Web.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using RestaurantMenuProject.Services.Data.Contracts;
    using RestaurantMenuProject.Web.Controllers.Api;
    using RestaurantMenuProject.Web.ViewModels;

    public class MenuController : Controller
    {
        private readonly IDrinkTypeService drinkTypeService;
        private readonly IDishTypeService dishTypeService;
        private readonly IDishService dishService;
        private readonly IDrinkService drinkService;
        private readonly ICommentService commentService;

        public MenuController(
            IDrinkTypeService drinkTypeService,
            IDishTypeService dishTypeService,
            IDishService dishService,
            IDrinkService drinkService,
            ICommentService commentService)
        {
            this.drinkTypeService = drinkTypeService;
            this.dishTypeService = dishTypeService;
            this.dishService = dishService;
            this.drinkService = drinkService;
            this.commentService = commentService;
        }

        public IActionResult DisplayFood(string type, string id)
        {
            if (id == null)
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

        public IActionResult DisplayDrink(string type, string id)
        {
            if (id == null)
            {
                var drinks = this.drinkService.GetAllDrinksByType(type);
                return this.View("DisplayDrinkType", drinks);
            }
            else
            {
                var drink = this.drinkService.GetDrinkItemViewModelById(id);
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

        public IActionResult Search(string searchTerm)
        {
            if (searchTerm == null)
            {
                return this.RedirectToAction("Index");
            }
            var dishes = this.dishService.GetDishViewModelBySearchTerm(searchTerm);
            var drinks = this.drinkService.GetAllDrinksBySearchTerm(searchTerm);

            var viewModel = new SearchViewModel()
            {
                SearchTerm = searchTerm,
                Dishes = dishes,
                Drinks = drinks,
            };

            return this.View(viewModel);
        }
    }
}
