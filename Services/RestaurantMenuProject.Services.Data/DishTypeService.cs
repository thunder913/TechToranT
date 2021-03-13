namespace RestaurantMenuProject.Services.Data
{
    using System.Collections.Generic;
    using System.Linq;

    using RestaurantMenuProject.Data.Common.Repositories;
    using RestaurantMenuProject.Data.Models;
    using RestaurantMenuProject.Services.Data.Contracts;
    using RestaurantMenuProject.Services.Mapping;
    using RestaurantMenuProject.Web.ViewModels;

    public class DishTypeService : IDishTypeService
    {
        private readonly IDeletableEntityRepository<DishType> dishTypeRepository;

        public DishTypeService(IDeletableEntityRepository<DishType> dishTypeRepository) 
        {
            this.dishTypeRepository = dishTypeRepository;
        }

        public ICollection<MenuItemViewModel> GetAllDishTypes()
        {
            return this.dishTypeRepository
                .AllAsNoTracking()
                .To<MenuItemViewModel>()
                .ToList();
        }

        public ICollection<FoodTypeViewModel> GetAllDishTypesWithId()
        {
            return this.dishTypeRepository
                .AllAsNoTracking()
                .To<FoodTypeViewModel>()
                .ToList();
        }

        public DishType GetDishTypeById(int id)
        {
            return this.dishTypeRepository
                .All()
                .First(x => x.Id == id);
        }
    }
}
