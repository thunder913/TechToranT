namespace RestaurantMenuProject.Services.Data
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using AutoMapper;
    using RestaurantMenuProject.Data.Common.Repositories;
    using RestaurantMenuProject.Data.Models;
    using RestaurantMenuProject.Services.Data.Contracts;
    using RestaurantMenuProject.Services.Mapping;
    using RestaurantMenuProject.Web.ViewModels;

    public class DrinkService : IDrinkService
    {
        private readonly IDeletableEntityRepository<Drink> drinkRepository;
        private readonly IIngredientService ingredientService;
        private readonly IFileService fileService;
        private readonly IDrinkTypeService drinkTypeService;

        public DrinkService(
            IDeletableEntityRepository<Drink> drinkRepository,
            IIngredientService ingredientService,
            IFileService fileService,
            IDrinkTypeService drinkTypeService)
        {
            this.drinkRepository = drinkRepository;
            this.ingredientService = ingredientService;
            this.fileService = fileService;
            this.drinkTypeService = drinkTypeService;
        }

        public async Task<Drink> AddDrink(AddDrinkViewModel drink, string wwwroot)
        {
            var mapper = AutoMapperConfig.MapperInstance;
            var drinkToAdd = mapper.Map<Drink>(drink);
            drinkToAdd.Ingredients = this.ingredientService.GetAllIngredientsByIds(drink.IngredientsId.ToArray()).ToArray();

            var drinkType = this.drinkTypeService.GetDrinkTypeById(drinkToAdd.DrinkTypeId);
            await this.drinkRepository.AddAsync(drinkToAdd);
            await this.drinkRepository.SaveChangesAsync();
            await this.fileService.SaveImage("Drinks", drinkType.Name, drinkToAdd.Id, drink.Image, wwwroot);
            return drinkToAdd;
        }

        public DrinkItemViewModel GetDrinkItemViewModelById(int id)
        {
            return this.drinkRepository
                    .AllAsNoTracking()
                    .Where(x => x.Id == id)
                    .To<DrinkItemViewModel>()
                    .First();
        }

        public Drink GetDrinkById(int id)
        {
            return this.drinkRepository
                .All()
                .FirstOrDefault(x => x.Id == id);
        }

        public ICollection<DrinkItemViewModel> GetAllDrinksByType(string drinkType)
        {
            return this.drinkRepository
                    .AllAsNoTracking()
                    .Where(x => x.DrinkType.Name == drinkType)
                    .To<DrinkItemViewModel>()
                    .ToList();
        }
    }
}
