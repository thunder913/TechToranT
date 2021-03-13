namespace RestaurantMenuProject.Services.Data
{
    using System.Linq;
    using System.Threading.Tasks;

    using RestaurantMenuProject.Data.Common.Repositories;
    using RestaurantMenuProject.Data.Models;
    using RestaurantMenuProject.Services.Data.Contracts;
    using RestaurantMenuProject.Web.ViewModels;

    public class DrinkService : IDrinkService
    {
        private readonly IDeletableEntityRepository<Drink> drinkRepository;
        private readonly IDrinkTypeService drinkTypeService;
        private readonly IIngredientService ingredientService;

        public DrinkService(IDeletableEntityRepository<Drink> drinkRepository, IDrinkTypeService drinkTypeService, IIngredientService ingredientService)
        {
            this.drinkRepository = drinkRepository;
            this.drinkTypeService = drinkTypeService;
            this.ingredientService = ingredientService;
        }

        public async Task<Drink> AddDrink(AddDrinkViewModel drink)
        {
            var drinkType = this.drinkTypeService.GetDrinkTypeById(drink.DrinkTypeId);

            var drinkToAdd = new Drink()
            {
                AdditionalInfo = drink.AdditionalInfo,
                AlchoholByVolume = drink.AlchoholByVolume,
                Name = drink.Name,
                Weight = drink.Weight,
                Price = drink.Price,
                DrinkTypeId = drinkType.Id,
                PackagingTypeId = drink.PackaginTypeId,
                Ingredients = this.ingredientService.GetAllIngredientsByIds(drink.IngredientsId.ToArray()).ToArray(), // TODO use AutoMapper
            };
            await this.drinkRepository.AddAsync(drinkToAdd);
            await this.drinkRepository.SaveChangesAsync();

            return drinkToAdd;
        }
    }
}
