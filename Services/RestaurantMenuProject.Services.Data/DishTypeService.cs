namespace RestaurantMenuProject.Services.Data
{
    using System.Collections.Generic;
    using System.Linq;

    using RestaurantMenuProject.Data.Common.Repositories;
    using RestaurantMenuProject.Data.Models;
    using RestaurantMenuProject.Services.Data.Contracts;
    using RestaurantMenuProject.Web.ViewModels;

    public class DishTypeService : IDishTypeService
    {
        private readonly IDeletableEntityRepository<DishType> dishRepository;

        public DishTypeService(IDeletableEntityRepository<DishType> dishRepository) 
        {
            this.dishRepository = dishRepository;
        }

        public ICollection<MenuItemViewModel> GetAllDishTypes()
        {
            return this.dishRepository.AllAsNoTracking().Select(x => new MenuItemViewModel() 
            {
                Description = x.Description,
                Name = x.Name,
            })
                .ToList();
        }

        public ICollection<FoodTypeViewModel> GetAllDishTypesWithId()
        {
            return this.dishRepository.AllAsNoTracking().Select(x => new FoodTypeViewModel()
            {
                Id = x.Id,
                Name = x.Name,
                Description = x.Description,
            })
            .ToList();
        }
    }
}
