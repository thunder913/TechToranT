namespace RestaurantMenuProject.Services.Data
{
    using System.Collections.Generic;
    using System.Linq;

    using RestaurantMenuProject.Data.Common.Repositories;
    using RestaurantMenuProject.Data.Models;
    using RestaurantMenuProject.Services.Data.Contracts;
    using RestaurantMenuProject.Services.Mapping;
    using RestaurantMenuProject.Web.ViewModels;

    public class DrinkTypeService : IDrinkTypeService
    {
        private readonly IDeletableEntityRepository<DrinkType> drinkTypeRepository;

        public DrinkTypeService(IDeletableEntityRepository<DrinkType> drinkTypeRepository)
        {
            this.drinkTypeRepository = drinkTypeRepository;
        }

        public ICollection<MenuItemViewModel> GetAllDrinkTypes()
        {
            return this.drinkTypeRepository
                .AllAsNoTracking()
                .To<MenuItemViewModel>()
                .ToList();
        }

        public ICollection<FoodTypeViewModel> GetAllDrinkTypesWithId()
        {
            return this.drinkTypeRepository.AllAsNoTracking()
                .To<FoodTypeViewModel>()
                .ToList();
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