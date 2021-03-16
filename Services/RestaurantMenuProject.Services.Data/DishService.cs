namespace RestaurantMenuProject.Services.Data
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using RestaurantMenuProject.Data.Common.Repositories;
    using RestaurantMenuProject.Data.Models;
    using RestaurantMenuProject.Services.Data.Contracts;
    using RestaurantMenuProject.Services.Mapping;
    using RestaurantMenuProject.Web.ViewModels;

    public class DishService : IDishService
    {
        private readonly IDeletableEntityRepository<Dish> dishRepository;
        private readonly IIngredientService ingredientService;
        private readonly IFileService fileService;
        private readonly IDishTypeService dishTypeService;

        public DishService(
            IDeletableEntityRepository<Dish> dishRepository,
            IIngredientService ingredientService,
            IFileService fileService,
            IDishTypeService dishTypeService)
        {
            this.dishRepository = dishRepository;
            this.ingredientService = ingredientService;
            this.fileService = fileService;
            this.dishTypeService = dishTypeService;
        }

        public async Task AddDish(AddDishViewModel dish, string wwwroot)
        {
            var mapper = AutoMapperConfig.MapperInstance;

            var dishToAdd = mapper.Map<Dish>(dish);
            dishToAdd.Ingredients = this.ingredientService.GetAllIngredientsByIds(dish.IngredientsId.ToArray()).ToArray();
            var dishType = this.dishTypeService.GetDishTypeById(dishToAdd.DishTypeId);
            await this.dishRepository.AddAsync(dishToAdd);
            await this.dishRepository.SaveChangesAsync();
            await this.fileService.SaveImage("Dishes", dishType.Name, dishToAdd.Id, dish.Image, wwwroot);

        }

        public void RemoveDish(Dish dish)
        {
            this.dishRepository.Delete(dish);
        }

        public Dish GetDishById(int id)
        {
            return this.dishRepository.AllAsNoTracking().Where(x => x.Id == id).FirstOrDefault();
        }

        public FoodItemViewModel GetDishAsFoodItemById(int id)
        {
            return this.dishRepository
                    .AllAsNoTracking()
                    .Where(x => x.Id == id)
                    .To<FoodItemViewModel>()
                    .FirstOrDefault();
        }

        public ICollection<FoodItemViewModel> GetAllDisheshWithDishTypeAsFoodItem(string dishType)
        {
            return this.dishRepository
                        .AllAsNoTracking()
                        .Where(x => x.DishType.Name == dishType)
                        .To<FoodItemViewModel>()
                        .ToList();
        }

        public AddDishViewModel GetAddDishViewModelById(int id)
        {
            return this.dishRepository
                        .AllAsNoTracking()
                        .Where(x => x.Id == id)
                        .To<AddDishViewModel>()
                        .FirstOrDefault();
        }
    }
}
