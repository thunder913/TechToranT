namespace RestaurantMenuProject.Web.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using RestaurantMenuProject.Services.Data.Contracts;
    using RestaurantMenuProject.Web.ViewModels;
    using System.Linq;
    using RestaurantMenuProject.Services.Mapping;
    using AutoMapper;
    using RestaurantMenuProject.Data.Models;

    public class ManageController : Controller
    {
        private readonly IDishTypeService dishTypeService;
        private readonly IIngredientService ingredientService;
        private readonly IDishService dishService;
        private readonly IDrinkTypeService drinkTypeService;
        private readonly IPackagingService packagingService;
        private readonly IAllergenService allergenService;

        public ManageController(IDishTypeService dishTypeService, IIngredientService ingredientService, 
            IDishService dishService, IDrinkTypeService drinkTypeService, IPackagingService packagingService,
            IAllergenService allergenService)
        {
            this.dishTypeService = dishTypeService;
            this.ingredientService = ingredientService;
            this.dishService = dishService;
            this.drinkTypeService = drinkTypeService;
            this.packagingService = packagingService;
            this.allergenService = allergenService;
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
                    this.SetValuesToDishViewModel(model);
                    return this.View("AddDish", model);
                case "Drink":
                    var drinkViewModel = new AddDrinkViewModel();
                    this.SetValuesToDrinkViewModel(drinkViewModel);
                    return this.View("AddDrink", drinkViewModel);
                case "Ingredient":
                    var ingredientViewModel = new AddIngredientViewModel();
                    this.SetValuesToIngredientViewModel(ingredientViewModel);
                    return this.View("AddIngredient", ingredientViewModel);
                case "Allergen":
                    return this.View("AddAllergen", new AllergenViewModel());
            }

            return this.View();
        }

        [HttpPost]
        public IActionResult AddDish(AddDishViewModel dish)
        {
            if (!this.ModelState.IsValid || dish.IngredientsId.Count() == 0)
            {
                this.SetValuesToDishViewModel(dish);
                return this.View(dish);
            }

            // TODO USE AUTOMAPPER AND MAKE IT ADD TO THE DATABASE, + ADD MORE CHECKS AND BETTER ERROR MESSAGES
            return this.RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult AddDrink(AddDrinkViewModel drink)
        {
            if (!this.ModelState.IsValid)
            {
                this.SetValuesToDrinkViewModel(drink);
                return this.View(drink);
            }

            return this.RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult AddIngredient(AddIngredientViewModel ingredient)
        {
            if (!this.ModelState.IsValid)
            {
                this.SetValuesToIngredientViewModel(ingredient);
                return this.View(ingredient);
            }

            return this.RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult AddAllergen(AllergenViewModel allergen)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View(allergen);
            }

            return this.RedirectToAction("Index");
        }

        public IActionResult Remove()
        {
            return this.View();
        }

        private void SetValuesToDishViewModel(AddDishViewModel addDishViewModel)
        {
            addDishViewModel.Ingredients = this.ingredientService.GetAllAsDishIngredientViewModel().Select(x => new SelectListItem(x.Name, x.Id.ToString())).ToList();
            addDishViewModel.DishType = this.dishTypeService.GetAllDishTypesWithId().Select(x => new SelectListItem(x.Name, x.Id.ToString())).ToList();
        }

        private void SetValuesToDrinkViewModel(AddDrinkViewModel addDrinkViewModel)
        {
            addDrinkViewModel.DrinkType = this.drinkTypeService.GetAllDrinkTypesWithId().Select(x => new SelectListItem(x.Name, x.Id.ToString())).ToList();
            addDrinkViewModel.PackagingType = this.packagingService.GetAllPackagingTypes().Select(x => new SelectListItem(x.Name, x.Id.ToString())).ToList();
            addDrinkViewModel.Ingredients = this.ingredientService.GetAllAsDishIngredientViewModel().Select(x => new SelectListItem(x.Name, x.Id.ToString())).ToList();
        }

        private void SetValuesToIngredientViewModel(AddIngredientViewModel addIngredientViewModel)
        {
            addIngredientViewModel.Allergens = this.allergenService.GetAllergensWithId().Select(x=> new SelectListItem(x.Name, x.Id.ToString())).ToList();
        }
    }
}

// TODO MAKE SERVICE FOR PACKAGING TYPE (SAME AS DRINKTYPESERVICE) AND FOR INGREDIENTS(WE ALREADT HAVE, JUST ADD IT TO THE VALUE)
// SEND THE ITEM TO THE VIEW