namespace RestaurantMenuProject.Web.Controllers
{
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;

    using AutoMapper;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using RestaurantMenuProject.Data.Models;
    using RestaurantMenuProject.Services.Data.Contracts;
    using RestaurantMenuProject.Services.Mapping;
    using RestaurantMenuProject.Web.ViewModels;

    public class ManageController : Controller
    {
        private readonly IDishTypeService dishTypeService;
        private readonly IIngredientService ingredientService;
        private readonly IDishService dishService;
        private readonly IDrinkTypeService drinkTypeService;
        private readonly IPackagingService packagingService;
        private readonly IAllergenService allergenService;
        private readonly IWebHostEnvironment webHostEnvironment;
        private readonly IDrinkService drinkService;

        public ManageController(
            IDishTypeService dishTypeService,
            IIngredientService ingredientService,
            IDishService dishService,
            IDrinkTypeService drinkTypeService,
            IPackagingService packagingService,
            IAllergenService allergenService,
            IWebHostEnvironment webHostEnvironment,
            IDrinkService drinkService)
        {
            this.dishTypeService = dishTypeService;
            this.ingredientService = ingredientService;
            this.dishService = dishService;
            this.drinkTypeService = drinkTypeService;
            this.packagingService = packagingService;
            this.allergenService = allergenService;
            this.webHostEnvironment = webHostEnvironment;
            this.drinkService = drinkService;
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
        public async Task<IActionResult> AddDish(AddDishViewModel dish)
        {
            if (!this.ModelState.IsValid)
            {
                this.SetValuesToDishViewModel(dish);
                return this.View(dish);
            }

            var dishType = this.dishTypeService.GetDishTypeById(dish.DishTypeId);

            var dishToAdd = new Dish()
            {
                AdditionalInfo = dish.AdditionalInfo,
                DishTypeId = dishType.Id,
                PrepareTime = dish.PrepareTime,
                Price = dish.Price,
                Weight = dish.Weight,
                Name = dish.Name,
                Ingredients = this.ingredientService.GetAllIngredientsByIds(dish.IngredientsId.ToArray()).ToArray(),
            };

            await this.dishService.AddDish(dishToAdd);

            // TODO USE AUTOMAPPER AND MAKE IT ADD TO THE DATABASE, + ADD MORE CHECKS AND BETTER ERROR MESSAGES
            // TODO Check if the file is the right format
            // TODO Send it into the Service
            await this.SaveImage("Menu", dishType.Name, dishToAdd.Id, dish.Image);

            return this.RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> AddDrink(AddDrinkViewModel drink)
        {
            if (!this.ModelState.IsValid)
            {
                this.SetValuesToDrinkViewModel(drink);
                return this.View(drink);
            }

            // TODO USE AUTOMAPPER AND MAKE IT ADD TO THE DATABASE, + ADD MORE CHECKS AND BETTER ERROR MESSAGES
            // TODO Check if the file is the right format
            var addedDrink = await this.drinkService.AddDrink(drink);

            await this.SaveImage("Drinks", addedDrink.DrinkType.Name, addedDrink.Id, drink.Image);

            return this.RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> AddIngredient(AddIngredientViewModel ingredient)
        {
            if (!this.ModelState.IsValid)
            {
                this.SetValuesToIngredientViewModel(ingredient);
                return this.View(ingredient);
            }

            var ingredientToAdd = new Ingredient()
            {
                Name = ingredient.Name,
                Allergens = this.allergenService.GetAllergensWithIds(ingredient.AllergensId.ToList()).ToList(),
            };

            await this.ingredientService.AddIngredient(ingredientToAdd);

            return this.RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> AddAllergen(AllergenViewModel allergen)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View(allergen);
            }

            var allergenToAdd = new Allergen()
            {
                Name = allergen.Name,
            };

            await this.allergenService.AddAllergen(allergenToAdd);

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
            addIngredientViewModel.Allergens = this.allergenService.GetAllergensWithId().Select(x => new SelectListItem(x.Name, x.Id.ToString())).ToList();
        }

        private async Task SaveImage(string itemCategory, string type, int id, IFormFile formFile)
        {
            var path = $"{this.webHostEnvironment.WebRootPath}/img/{itemCategory}/{type}";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            using (FileStream fs = new FileStream(path + $"/{id}.jpg", FileMode.Create))
            {
                await formFile.CopyToAsync(fs);
            }
        }
    }
}

// TODO MAKE SERVICE FOR PACKAGING TYPE (SAME AS DRINKTYPESERVICE) AND FOR INGREDIENTS(WE ALREADT HAVE, JUST ADD IT TO THE VALUE)
// SEND THE ITEM TO THE VIEW
