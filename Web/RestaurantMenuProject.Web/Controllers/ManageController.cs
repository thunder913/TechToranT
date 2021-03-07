namespace RestaurantMenuProject.Web.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using RestaurantMenuProject.Services.Data.Contracts;
    using RestaurantMenuProject.Web.ViewModels;
    using System.Linq;
    using RestaurantMenuProject.Services.Mapping;

    public class ManageController : Controller
    {
        private readonly IDishTypeService dishTypeService;
        private readonly IIngredientService ingredientService;
        private readonly IDishService dishService;
        private readonly IDrinkTypeService drinkTypeService;
        private readonly IPackagingService packagingService;

        public ManageController(IDishTypeService dishTypeService, IIngredientService ingredientService, 
            IDishService dishService, IDrinkTypeService drinkTypeService, IPackagingService packagingService)
        {
            this.dishTypeService = dishTypeService;
            this.ingredientService = ingredientService;
            this.dishService = dishService;
            this.drinkTypeService = drinkTypeService;
            this.packagingService = packagingService;
        }

        public IActionResult Index()
        {
            return this.View();
        }

        [HttpPost]
        public IActionResult Add(string type)
        {
            switch (type)
            {
                case "Dish":
                    var model = new AddDishViewModel();
                    this.SetConstantsToDishViewModel(model);
                    return this.View("AddDish", model);
                case "Drink":
                    var drinkViewModel = new AddDrinkViewModel();
                    this.SetConstantsToDrinkViewModel(drinkViewModel);
                    return this.View("AddDrink", drinkViewModel);
            }

            return this.View();
        }

        public IActionResult AddDish(AddDishViewModel dish)
        {
            if (!this.ModelState.IsValid || dish.IngredientsId.Count() == 0)
            {
                this.SetConstantsToDishViewModel(dish);
                return this.View(dish);
            }

            // TODO USE AUTOMAPPER AND MAKE IT ADD TO THE DATABASE, + ADD MORE CHECKS AND BETTER ERROR MESSAGES
            return this.RedirectToAction("Index");
        }

        public IActionResult Remove()
        {
            return this.View();
        }

        private void SetConstantsToDishViewModel(AddDishViewModel addDishViewModel)
        {
            addDishViewModel.Ingredients = this.ingredientService.GetAllAsDishIngredientViewModel().Select(x => new SelectListItem(x.Name, x.Id.ToString())).ToList();
            addDishViewModel.DishType = this.dishTypeService.GetAllDishTypesWithId().Select(x => new SelectListItem(x.Name, x.Id.ToString())).ToList();
        }

        private void SetConstantsToDrinkViewModel(AddDrinkViewModel addDrinkViewModel)
        {
            addDrinkViewModel.DrinkType = this.drinkTypeService.GetAllDrinkTypesWithId().Select(x => new SelectListItem(x.Name, x.Id.ToString())).ToList();
            addDrinkViewModel.PackagingType = this.packagingService.GetAllPackagingTypes().Select(x => new SelectListItem(x.Name, x.Id.ToString())).ToList();
            addDrinkViewModel.Ingredients = this.ingredientService.GetAllAsDishIngredientViewModel().Select(x => new SelectListItem(x.Name, x.Id.ToString())).ToList();
        }
    }
}

// TODO MAKE SERVICE FOR PACKAGING TYPE (SAME AS DRINKTYPESERVICE) AND FOR INGREDIENTS(WE ALREADT HAVE, JUST ADD IT TO THE VALUE)
// SEND THE ITEM TO THE VIEW