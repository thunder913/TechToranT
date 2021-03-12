namespace RestaurantMenuProject.Services.Data
{
    using System.Collections.Generic;
    using System.Linq;

    using RestaurantMenuProject.Data.Common.Repositories;
    using RestaurantMenuProject.Data.Models;
    using RestaurantMenuProject.Services.Data.Contracts;
    using RestaurantMenuProject.Web.ViewModels;

    public class DrinkTypeService : IDrinkTypeService
    {
        private readonly IDeletableEntityRepository<DrinkType> drinkTypeRepository;
        private readonly IDeletableEntityRepository<Drink> drinkRepository;

        public DrinkTypeService(IDeletableEntityRepository<DrinkType> drinkTypeRepository, IDeletableEntityRepository<Drink> drinkRepository)
        {
            this.drinkTypeRepository = drinkTypeRepository;
            this.drinkRepository = drinkRepository;
        }

        public ICollection<DrinkItemViewModel> GetAllDrinksByType(string drinkType)
        {
            return this.drinkRepository
                    .AllAsNoTracking()
                    .Where(x => x.DrinkType.Name == drinkType)
                    .Select(x => new DrinkItemViewModel()
                    {
                        AdditionalInfo = x.AdditionalInfo,
                        AlchoholByVolume = x.AlchoholByVolume,
                        DrinkType = x.DrinkType,
                        Id = x.Id,
                        Name = x.Name,
                        PackagingType = x.PackagingType,
                        Price = x.Price,
                        Weight = x.Weight,
                        Ingredients = x.Ingredients.Select(i => new IngredientViewModel()
                        {
                            Allergens = i.Allergens.Select(a => new AllergenViewModel()
                            {
                                Id = a.Id,
                                Name = a.Name,
                            }).ToList(),
                            Name = i.Name,
                        }).ToList(),
                    })
                    .ToList();
        }

        public ICollection<MenuItemViewModel> GetAllDrinkTypes()
        {
            return this.drinkTypeRepository.AllAsNoTracking().Select(x => new MenuItemViewModel()
            {
                Description = x.Description,
                Name = x.Name,
            }).ToList();
        }

        public ICollection<FoodTypeViewModel> GetAllDrinkTypesWithId()
        {
            return this.drinkTypeRepository.AllAsNoTracking().Select(x => new FoodTypeViewModel()
            {
                Id = x.Id,
                Name = x.Name,
                Description = x.Description,
            })
        .ToList();
        }

        public DrinkItemViewModel GetDrinkById(int id)
        {
            return this.drinkRepository
                    .AllAsNoTracking()
                    .Where(x => x.Id == id)
                    .Select(x => new DrinkItemViewModel()
                    {
                        AdditionalInfo = x.AdditionalInfo,
                        AlchoholByVolume = x.AlchoholByVolume,
                        DrinkType = x.DrinkType,
                        Id = x.Id,
                        Name = x.Name,
                        PackagingType = x.PackagingType,
                        Price = x.Price,
                        Weight = x.Weight,
                        Ingredients = x.Ingredients.Select(i => new IngredientViewModel()
                        {
                            Allergens = i.Allergens.Select(a => new AllergenViewModel()
                            {
                                Id = a.Id,
                                Name = a.Name,
                            }).ToList(),
                            Name = i.Name,
                        }).ToList(),
                    })
                    .First();
        }

        public DrinkType GetDrinkTypeById(int id)
        {
            return this.drinkTypeRepository
                    .All()
                    .Where(x => x.Id == id)
                    .First();
        }
    }
}

// TODO: use automapper instead of Selects.. (also in dishService)
