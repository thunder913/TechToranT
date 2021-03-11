namespace RestaurantMenuProject.Services.Data
{
    using System.Collections.Generic;
    using System.Linq;

    using RestaurantMenuProject.Data.Common.Repositories;
    using RestaurantMenuProject.Data.Models;
    using RestaurantMenuProject.Services.Data.Contracts;
    using RestaurantMenuProject.Web.ViewModels;

    // TODO rename the service everywhere to dishService
    public class DishTypeService : IDishTypeService
    {
        private readonly IDeletableEntityRepository<DishType> dishTypeRepository;
        private readonly IDeletableEntityRepository<Dish> dishRepository;

        public DishTypeService(IDeletableEntityRepository<DishType> dishTypeRepository, IDeletableEntityRepository<Dish> dishRepository) 
        {
            this.dishTypeRepository = dishTypeRepository;
            this.dishRepository = dishRepository;
        }

        public ICollection<FoodItemViewModel> GetAllDisheshWithDishType(string dishType)
        {
            return this.dishRepository
                        .AllAsNoTracking()
                        .Where(x => x.DishType.Name == dishType)
                        .Select(x => new FoodItemViewModel()
                        {
                            Id = x.Id,
                            DishType = x.DishType,
                            Name = x.Name,
                            PrepareTime = x.PrepareTime,
                            Price = x.Price,
                            Weight = x.Weight,
                        })
                        .ToList();
        }

        public ICollection<MenuItemViewModel> GetAllDishTypes()
        {
            return this.dishTypeRepository.AllAsNoTracking().Select(x => new MenuItemViewModel() 
            {
                Description = x.Description,
                Name = x.Name,
            })
                .ToList();
        }

        public ICollection<FoodTypeViewModel> GetAllDishTypesWithId()
        {
            return this.dishTypeRepository.AllAsNoTracking().Select(x => new FoodTypeViewModel()
            {
                Id = x.Id,
                Name = x.Name,
                Description = x.Description,
            })
            .ToList();
        }

        public DishType GetDishTypeById(int id)
        {
            return this.dishTypeRepository.AllAsNoTracking().First(x => x.Id == id);
        }

        public FoodItemViewModel GetDishWithId(int id)
        {
            return this.dishRepository
                    .AllAsNoTracking()
                    .Where(x => x.Id == id)
                    .Select(x => new FoodItemViewModel()
                    {
                        Id = x.Id,
                        DishType = x.DishType,
                        Name = x.Name,
                        PrepareTime = x.PrepareTime,
                        Price = x.Price,
                        Weight = x.Weight,
                        AdditionalInfo = x.AdditionalInfo,
                        Ingredients = x.Ingredients.Select(i => new IngredientViewModel()
                        {
                            Name = i.Name,
                            Allergens = i.Allergens.Select(a => new AllergenViewModel()
                            {
                                Id = a.Id,
                                Name = a.Name,
                            })
                            .ToList(),
                        }).ToList(),
                    })
                    .FirstOrDefault();
        }
    }
}
