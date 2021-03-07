namespace RestaurantMenuProject.Web.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using RestaurantMenuProject.Services.Data.Contracts;
    using RestaurantMenuProject.Web.ViewModels;
    using System.Linq;

    public class ManageController : Controller
    {
        private readonly IDishTypeService dishTypeService;
        private readonly IIngredientService ingredientService;

        public ManageController(IDishTypeService dishTypeService, IIngredientService ingredientService)
        {
            this.dishTypeService = dishTypeService;
            this.ingredientService = ingredientService;
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
                    model.Ingredients = this.ingredientService.GetAllAsDishIngredientViewModel().Select(x => new SelectListItem(x.Name,x.Id.ToString())).ToList();
                    model.DishType = this.dishTypeService.GetAllDishTypesWithId().Select(x => new SelectListItem(x.Name, x.Id.ToString())).ToList();
                    return this.View("AddDish", model);
            }

            return this.View();
        }

        public IActionResult AddDish(string name, decimal price, double weight, string additionalInfo, int dishTypeId, int[]? ingredients)
        {
            if (!this.ModelState.IsValid || ingredients.Count() == 0)
            {
                return this.View();
            }

            return this.RedirectToAction("Index");
        }

        public IActionResult Remove()
        {
            return this.View();
        }
    }
}
